from PIL import Image
import numpy as np
import json
import colorsys

# ======================
# CONFIG
# ======================
INPUT_IMAGE = "input.png"
COLOR_CONFIG = "color_config.json"
TARGET_SIZE = 64
OUT_JSON = "output.json"

# ======================
# VALIDATION CONFIG
# ======================
MIN_REGION_PIXELS = 20
MAX_OTHER_RATIO = 0.15
MIN_TOTAL_RATIO = 0.25
MAIN_REGION_RATIO = 0.15

# ======================
# LOAD COLOR CONFIG
# ======================
with open(COLOR_CONFIG, "r", encoding="utf-8") as f:
    CFG = json.load(f)

IGNORE = CFG["ignore"]
REGIONS = CFG["regions"]

# ======================
# HSV UTILS
# ======================
def rgb_to_hsv(rgb):
    r, g, b = [x / 255.0 for x in rgb]
    return colorsys.rgb_to_hsv(r, g, b)

def classify_color(rgb):
    h, s, v = rgb_to_hsv(rgb)
    h_deg = h * 360

    # -------- IGNORE RULES --------
    if v < IGNORE["black_v"]:
        return None

    if s < IGNORE["white_s"] and v > IGNORE["white_v"]:
        return None

    # -------- REGION RULES --------
    for r in REGIONS:
        if r["h_min"] <= h_deg <= r["h_max"] and s >= r["s_min"]:
            return r["name"]

    return "other"

# ======================
# BACKGROUND DETECTION
# ======================
def detect_background_color(arr):
    h, w, _ = arr.shape
    samples = [
        arr[0, 0],
        arr[0, w - 1],
        arr[h - 1, 0],
        arr[h - 1, w - 1]
    ]
    avg = np.mean(samples, axis=0)
    return tuple(int(x) for x in avg)

# ======================
# LOAD IMAGE
# ======================
img = Image.open(INPUT_IMAGE).convert("RGB")
img = img.resize((TARGET_SIZE, TARGET_SIZE), Image.NEAREST)
arr = np.array(img)

h, w, _ = arr.shape

# detect background visual color
background_color = detect_background_color(arr)

# ======================
# BUILD REGION MAP
# ======================
region_grid = [["none" for _ in range(w)] for _ in range(h)]
pixels = []
region_stats = {}

for y in range(h):
    for x in range(w):
        rgb = tuple(arr[y, x])
        region = classify_color(rgb)

        # background / ignored pixels
        if region is None:
            continue

        region_grid[y][x] = region
        pixels.append({
            "x": x,
            "y": y,
            "region": region
        })

        region_stats[region] = region_stats.get(region, 0) + 1

# ======================
# LEVEL VALIDATION
# ======================
def validate_level(region_stats, w, h):
    total_pixels = w * h
    region_pixels = sum(region_stats.values())
    other_pixels = region_stats.get("other", 0)

    errors = []
    warnings = []

    if region_pixels / total_pixels < MIN_TOTAL_RATIO:
        errors.append("Too few region pixels (image mostly ignored)")

    if region_pixels > 0 and other_pixels / region_pixels > MAX_OTHER_RATIO:
        warnings.append("Too many 'other' pixels")

    for r, count in region_stats.items():
        if r != "other" and count < MIN_REGION_PIXELS:
            warnings.append(f"Region '{r}' too small ({count} px)")

    main_ok = any(
        count / total_pixels >= MAIN_REGION_RATIO
        for r, count in region_stats.items()
        if r != "other"
    )

    if not main_ok:
        errors.append("No main region large enough")

    return errors, warnings

errors, warnings = validate_level(region_stats, w, h)

# ======================
# SAVE JSON
# ======================
data = {
    "width": w,
    "height": h,

    # 👇 background visual only (NO gameplay)
    "background": {
        "color": background_color
    },

    "regions": pixels,
    "region_grid": region_grid,
    "stats": region_stats,
    "validation": {
        "errors": errors,
        "warnings": warnings,
        "ok": len(errors) == 0
    }
}

with open(OUT_JSON, "w", encoding="utf-8") as f:
    json.dump(data, f, indent=2)

# ======================
# REPORT
# ======================
print("=== LEVEL CHECK ===")

if errors:
    print("❌ ERRORS:")
    for e in errors:
        print("  -", e)
else:
    print("✅ No critical errors")

if warnings:
    print("⚠️ WARNINGS:")
    for wng in warnings:
        print("  -", wng)
else:
    print("✅ No warnings")

if errors:
    print("⛔ Level NOT OK – do not import to Unity")
else:
    print("🎮 Level OK – ready for Unity")

print("\nSaved:", OUT_JSON)
print("Background color:", background_color)
print("Regions:")
for k, v in region_stats.items():
    print(f"  - {k}: {v} pixels")
