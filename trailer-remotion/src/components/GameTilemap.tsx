import React, { useMemo } from "react";
import { AbsoluteFill, Img, staticFile } from "remotion";

function cellHash(col: number, row: number, seed = 1): number {
  const n = Math.sin(col * 127.1 + row * 311.7 + seed * 74.7) * 43758.5453;
  return n - Math.floor(n);
}

export interface GroundedSpriteProps {
  src: string;
  x: number;
  y: number;
  size: number;
  shadowWidth?: number;
  shadowHeight?: number;
  shadowOffsetY?: number;
  flipX?: boolean;
  children?: React.ReactNode;
  zIndexOverride?: number;
}

/**
 * Renders a top-down pixel-art sprite grounded onto the Tilemap with a soft
 * contact shadow under its base and automatic Y-sort depth ordering (like YSortCamera in Unity).
 */
export const GroundedSprite: React.FC<GroundedSpriteProps> = ({
  src,
  x,
  y,
  size,
  shadowWidth,
  shadowHeight,
  shadowOffsetY = 0,
  flipX = false,
  children,
  zIndexOverride,
}) => {
  const sw = shadowWidth ?? Math.round(size * 0.68);
  const sh = shadowHeight ?? Math.round(size * 0.22);
  const z = zIndexOverride ?? Math.round(y + size * 0.85);

  return (
    <div
      style={{
        position: "absolute",
        left: Math.round(x - size / 2),
        top: Math.round(y - size / 2),
        width: size,
        height: size,
        zIndex: z,
        pointerEvents: "none",
      }}
    >
      {/* Ground Contact Shadow under the base/feet/wheels */}
      <div
        style={{
          position: "absolute",
          left: Math.round((size - sw) / 2),
          bottom: Math.round(size * 0.06 - shadowOffsetY),
          width: sw,
          height: sh,
          borderRadius: "50%",
          background:
            "radial-gradient(ellipse at center, rgba(0,0,0,0.62) 0%, rgba(0,0,0,0.28) 58%, transparent 80%)",
        }}
      />
      <Img
        src={staticFile(src)}
        style={{
          position: "absolute",
          inset: 0,
          width: "100%",
          height: "100%",
          objectFit: "contain",
          imageRendering: "pixelated",
          transform: flipX ? "scaleX(-1)" : undefined,
        }}
      />
      {children}
    </div>
  );
};

/**
 * Authentic multi-tile Space Center / Engineering Hangar Tilemap (01_Hub & 02_Workshop).
 * Uses seamless metallic laboratory floor panels (`floor_panels`, `floor_rivets`, `floor_grate`)
 * and a connected 3x3 vehicle assembly pad (`pavimento-1-1..3-3`) in the Workshop Hangar.
 */
export const HubTilemap: React.FC<{ variant?: "hub" | "workshop" }> = ({
  variant = "hub",
}) => {
  const cols = 38;
  const rows = 22;
  const cellSize = 64;

  const cells = useMemo(() => {
    const out: { col: number; row: number; tile: string }[] = [];
    for (let r = 0; r < rows; r++) {
      for (let c = 0; c < cols; c++) {
        const h = cellHash(c, r, variant === "hub" ? 11 : 29);
        let tile = "game-assets/Tilesets/Hub/LabTilesPNG/floor_panels.png";

        if (variant === "hub") {
          if (
            r === 8 ||
            r === 9 ||
            ((c === 14 || c === 23) && r >= 3 && r <= 8)
          ) {
            tile = "game-assets/Tilesets/Hub/LabTilesPNG/floor_grate.png";
          } else if (r === 7 || r === 10 || h > 0.68) {
            tile = "game-assets/Tilesets/Hub/LabTilesPNG/floor_rivets.png";
          } else {
            tile = "game-assets/Tilesets/Hub/LabTilesPNG/floor_panels.png";
          }
        } else {
          // Workshop Hangar Bay: connected 3x3 pavement pad under the Rover (cols 10..12, rows 8..10 -> center x=608, y=544)
          // framed by a grate service perimeter (cols 9..13, rows 7..11) and rollout track to the right
          if (c >= 10 && c <= 12 && r >= 8 && r <= 10) {
            const pr = r - 7; // 1..3
            const pc = c - 9; // 1..3
            tile = `game-assets/Tilesets/Hub/pavimento-${pr}-${pc}.png`;
          } else if (
            (c >= 9 && c <= 13 && (r === 7 || r === 11)) ||
            (r >= 7 && r <= 11 && (c === 9 || c === 13)) ||
            ((r === 8 || r === 9) && c >= 13 && c <= 22)
          ) {
            tile = "game-assets/Tilesets/Hub/LabTilesPNG/floor_grate.png";
          } else if (h > 0.65) {
            tile = "game-assets/Tilesets/Hub/LabTilesPNG/floor_rivets.png";
          } else {
            tile = "game-assets/Tilesets/Hub/LabTilesPNG/floor_panels.png";
          }
        }
        out.push({ col: c, row: r, tile });
      }
    }
    return out;
  }, [variant]);

  return (
    <div
      style={{
        position: "absolute",
        left: -128,
        top: -64,
        width: cols * cellSize,
        height: rows * cellSize,
      }}
    >
      {cells.map((cell, i) => (
        <Img
          key={i}
          src={staticFile(cell.tile)}
          style={{
            position: "absolute",
            left: cell.col * cellSize,
            top: cell.row * cellSize,
            width: cellSize + 1,
            height: cellSize + 1,
            imageRendering: "pixelated",
          }}
        />
      ))}
    </div>
  );
};

/**
 * Authentic Mars Jezero Crater Tilemap (04_Mars) using seamless color-matched zones:
 * - Stepped river delta channel: `sand_a`, `sand_b`, `sand_c` (identical #c86535 base, zero grid seams)
 * - Main Martian plain: `dirt_a`, `dirt_b` (identical #8d3822 base, zero grid seams)
 * - Ancient cracked mudflat & rocky hazard zone: `mud_a..f`, `mud_hex` (identical #582014 base, zero grid seams)
 * - Sparse natural craters (`h > 0.94`) placed only on interior sand/dirt cells.
 */
export const MarsTilemap: React.FC = () => {
  const cols = 38;
  const rows = 22;
  const cellSize = 64;

  const cells = useMemo(() => {
    const out: { col: number; row: number; tile: string }[] = [];

    const sandBase = [
      "game-assets/Tilesets/Mars/MarsTilesPNG/sand_a.png",
      "game-assets/Tilesets/Mars/MarsTilesPNG/sand_b.png",
      "game-assets/Tilesets/Mars/MarsTilesPNG/sand_c.png",
    ];
    const dirtBase = [
      "game-assets/Tilesets/Mars/MarsTilesPNG/dirt_a.png",
      "game-assets/Tilesets/Mars/MarsTilesPNG/dirt_b.png",
    ];
    const mudBase = [
      "game-assets/Tilesets/Mars/MarsTilesPNG/mud_a.png",
      "game-assets/Tilesets/Mars/MarsTilesPNG/mud_b.png",
      "game-assets/Tilesets/Mars/MarsTilesPNG/mud_c.png",
      "game-assets/Tilesets/Mars/MarsTilesPNG/mud_d.png",
      "game-assets/Tilesets/Mars/MarsTilesPNG/mud_e.png",
      "game-assets/Tilesets/Mars/MarsTilesPNG/mud_f.png",
      "game-assets/Tilesets/Mars/MarsTilesPNG/mud_hex.png",
    ];

    for (let r = 0; r < rows; r++) {
      for (let c = 0; c < cols; c++) {
        const h = cellHash(c, r, 42);
        const riverCenter =
          8.6 + Math.sin(c * 0.24) * 1.9 + (c > 18 ? (c - 18) * 0.14 : 0);
        const riverHalfWidth = c > 18 ? 2.3 + (c - 18) * 0.14 : 2.0;
        const dist = Math.abs(r - riverCenter);

        let tile = dirtBase[Math.floor(h * dirtBase.length)];

        if (dist < riverHalfWidth) {
          if (dist < riverHalfWidth - 0.8 && h > 0.93) {
            tile =
              h > 0.965
                ? "game-assets/Tilesets/Mars/MarsTilesPNG/crater_a.png"
                : "game-assets/Tilesets/Mars/MarsTilesPNG/crater_b.png";
          } else {
            tile = sandBase[Math.floor(h * sandBase.length)];
          }
        } else if (c >= 12 && c <= 25 && r >= 12 && r <= 17) {
          tile = mudBase[Math.floor(h * mudBase.length)];
        } else {
          tile = dirtBase[Math.floor(h * dirtBase.length)];
        }

        out.push({ col: c, row: r, tile });
      }
    }
    return out;
  }, []);

  return (
    <div
      style={{
        position: "absolute",
        left: -128,
        top: -64,
        width: cols * cellSize,
        height: rows * cellSize,
      }}
    >
      {cells.map((cell, i) => (
        <Img
          key={i}
          src={staticFile(cell.tile)}
          style={{
            position: "absolute",
            left: cell.col * cellSize,
            top: cell.row * cellSize,
            width: cellSize + 1,
            height: cellSize + 1,
            imageRendering: "pixelated",
          }}
        />
      ))}
    </div>
  );
};

/**
 * Authentic Lunar South Pole Tilemap (04_Moon) using seamless color-matched zones:
 * - Sunlit Lunar Highlands (left): `sand_a`, `sand_b`, `sand_c` (identical #8e96a0 base, zero grid seams)
 * - Stepped Crater Rim Slope (middle): `dirt_a`, `dirt_b`
 * - Permanent Shadow Crater Basin (-240 C, right): `mud_a..f`, `mud_hex` (identical #1d242d base, zero grid seams)
 */
export const MoonTilemap: React.FC = () => {
  const cols = 38;
  const rows = 22;
  const cellSize = 64;

  const cells = useMemo(() => {
    const out: {
      col: number;
      row: number;
      tile: string;
      shadowDepth: number;
    }[] = [];

    const sunlitBase = [
      "game-assets/Tilesets/Moon/MoonTilesPNG/sand_a.png",
      "game-assets/Tilesets/Moon/MoonTilesPNG/sand_b.png",
      "game-assets/Tilesets/Moon/MoonTilesPNG/sand_c.png",
    ];
    const rimBase = [
      "game-assets/Tilesets/Moon/MoonTilesPNG/dirt_a.png",
      "game-assets/Tilesets/Moon/MoonTilesPNG/dirt_b.png",
    ];
    const shadowBase = [
      "game-assets/Tilesets/Moon/MoonTilesPNG/mud_a.png",
      "game-assets/Tilesets/Moon/MoonTilesPNG/mud_b.png",
      "game-assets/Tilesets/Moon/MoonTilesPNG/mud_c.png",
      "game-assets/Tilesets/Moon/MoonTilesPNG/mud_d.png",
      "game-assets/Tilesets/Moon/MoonTilesPNG/mud_e.png",
      "game-assets/Tilesets/Moon/MoonTilesPNG/mud_f.png",
      "game-assets/Tilesets/Moon/MoonTilesPNG/mud_hex.png",
    ];

    const cx = 23.5;
    const cy = 9.5;

    for (let r = 0; r < rows; r++) {
      for (let c = 0; c < cols; c++) {
        const h = cellHash(c, r, 77);
        const dx = (c - cx) / 10.5;
        const dy = (r - cy) / 7.2;
        const distSq = dx * dx + dy * dy + (h - 0.5) * 0.08;

        let tile = sunlitBase[Math.floor(h * sunlitBase.length)];
        let shadowDepth = 0;

        if (distSq < 0.68) {
          tile = shadowBase[Math.floor(h * shadowBase.length)];
          shadowDepth = 0.52;
        } else if (distSq < 0.96) {
          tile = rimBase[Math.floor(h * rimBase.length)];
          shadowDepth = 0.25;
        } else {
          tile = sunlitBase[Math.floor(h * sunlitBase.length)];
        }

        out.push({ col: c, row: r, tile, shadowDepth });
      }
    }
    return out;
  }, []);

  return (
    <div
      style={{
        position: "absolute",
        left: -128,
        top: -64,
        width: cols * cellSize,
        height: rows * cellSize,
      }}
    >
      {cells.map((cell, i) => (
        <div
          key={i}
          style={{
            position: "absolute",
            left: cell.col * cellSize,
            top: cell.row * cellSize,
            width: cellSize + 1,
            height: cellSize + 1,
          }}
        >
          <Img
            src={staticFile(cell.tile)}
            style={{
              width: "100%",
              height: "100%",
              imageRendering: "pixelated",
            }}
          />
          {cell.shadowDepth > 0 && (
            <AbsoluteFill
              style={{
                backgroundColor: `rgba(6, 14, 34, ${cell.shadowDepth})`,
              }}
            />
          )}
        </div>
      ))}
    </div>
  );
};
