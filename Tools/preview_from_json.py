import json
import random
from PIL import Image

# ======================
# CONFIG
# ======================
INPUT_JSON = "output.json"
SCALE = 8  # phóng to để nhìn
OUT_PREVIEW = "unity_like_preview.png"

# Palette giả lập Unity (bạn đổi tuỳ ý)
PALETTES = {
    "orange": [(255, 160, 80), (220, 120, 60)],
    "red":    [(230, 90, 90), (180, 60, 60)],
    "yellow": [(255, 230, 120), (230, 190, 90)],
    "green":  [(100, 220, 120), (60, 180, 90)],
    "purple": [(180, 120, 220), (140, 90, 180)],
    "other":  [(120, 120, 120)]
}

# ======================
# LOAD JSON
# ======================
with open(INPUT_JSON, "r", encoding="utf-8") as f:
    data = json.load(f)

w = data["width"]
h = data["height"]
grid = data["region_grid"]

# ======================
# RENDER
# ======================
img = Image.new("RGB", (w, h), (30, 30, 30))
px = img.load()

for y in range(h):
    for x in range(w):
        region = grid[y][x]
        if region == "none":
            continue

        palette = PALETTES.get(region, PALETTES["other"])
        color = random.choice(palette)

        px[x, y] = color

# scale up for viewing
img = img.resize((w * SCALE, h * SCALE), Image.NEAREST)
img.save(OUT_PREVIEW)
img.show()

print("✅ Preview saved:", OUT_PREVIEW)
