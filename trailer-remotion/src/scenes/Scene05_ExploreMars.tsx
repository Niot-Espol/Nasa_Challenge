import React from "react";
import {
  AbsoluteFill,
  Img,
  interpolate,
  spring,
  staticFile,
  useCurrentFrame,
  useVideoConfig,
} from "remotion";
import { PixelSprite } from "../components/PixelSprite";
import { FONTS } from "../styles/theme";
import {
  DiagonalWipeIn,
  ForegroundParticles,
  ImpactFlash,
  TacticalHudFrame,
} from "../components/CinematicEffects";

export const Scene05_ExploreMars: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  // Dynamic Camera Tracking across the EXPANDED REAL GAME SCENARIO 04_Mars
  const camScale = interpolate(frame, [0, 90, 180], [1.04, 1.15, 1.24], {
    extrapolateRight: "clamp",
  });
  const camX = interpolate(frame, [0, 180], [90, -110], {
    extrapolateRight: "clamp",
  });
  const camY = interpolate(frame, [0, 180], [30, -40], {
    extrapolateRight: "clamp",
  });

  const roverX = interpolate(frame, [0, 180], [360, 1140], {
    extrapolateRight: "clamp",
  });
  const roverY = interpolate(frame, [0, 90, 180], [560, 470, 440], {
    extrapolateRight: "clamp",
  });

  const driveIdx = Math.floor(frame / 3) % 4;
  const roverSprite = `game-assets/Sprites/Rover/DrivingV2/East/drive_east_00${driveIdx}.png`;
  const scanIdx = Math.floor(frame / 3) % 4;

  const isStorm = frame >= 105;
  const stampPop = spring({
    frame: frame - 8,
    fps,
    config: { damping: 12, stiffness: 170 },
  });

  return (
    <AbsoluteFill style={{ backgroundColor: "#3b170e", overflow: "hidden" }}>
      <DiagonalWipeIn color="#f97316" />
      <ImpactFlash triggerFrame={105} duration={8} color="#f97316" maxOpacity={0.65} />

      {/* REAL GAME SCENARIO: 04_Mars Expanded Tilemap (MarsTilesPNG + Relief + Craters + SciencePoints) */}
      <AbsoluteFill
        style={{
          transform: `translate(${camX}px, ${camY}px) scale(${camScale})`,
          transformOrigin: "center center",
        }}
      >
        {/* Base Martian Regolith Tilemap (dirt_a.png) */}
        <AbsoluteFill
          style={{
            backgroundImage: `url(${staticFile(
              "game-assets/Tilesets/Mars/MarsTilesPNG/dirt_a.png"
            )})`,
            backgroundSize: "96px 96px",
            imageRendering: "pixelated",
          }}
        />

        {/* Ancient River Delta Channel & Sedimentary Sand Band (sand_a.png) */}
        <div
          style={{
            position: "absolute",
            left: -100,
            right: -100,
            top: 390,
            height: 290,
            transform: "rotate(-6deg)",
            backgroundImage: `url(${staticFile(
              "game-assets/Tilesets/Mars/MarsTilesPNG/sand_a.png"
            )})`,
            backgroundSize: "96px 96px",
            imageRendering: "pixelated",
            borderTop: "6px dashed rgba(251,146,60,0.45)",
            borderBottom: "6px dashed rgba(251,146,60,0.45)",
          }}
        />

        {/* Rocky Hazard Zone (rocky_ground.png) */}
        <div
          style={{
            position: "absolute",
            left: 740,
            top: 640,
            width: 580,
            height: 260,
            backgroundImage: `url(${staticFile(
              "game-assets/Tilesets/Mars/MarsTilesPNG/rocky_ground.png"
            )})`,
            backgroundSize: "96px 96px",
            imageRendering: "pixelated",
            border: "3px solid rgba(234,88,12,0.5)",
          }}
        />

        {/* Top & Border Canyon Mesas & Cliffs (Real Game Sprites/Mars/Relief/*) */}
        <div style={{ position: "absolute", left: 60, top: 80 }}>
          <PixelSprite src="game-assets/Sprites/Mars/Relief/meseta-grande.png" size={280} />
        </div>
        <div style={{ position: "absolute", left: 480, top: 60 }}>
          <PixelSprite src="game-assets/Sprites/Mars/Relief/cantil-largo.png" size={260} />
        </div>
        <div style={{ position: "absolute", left: 960, top: 75 }}>
          <PixelSprite src="game-assets/Sprites/Mars/Relief/meseta-alta.png" size={270} />
        </div>
        <div style={{ position: "absolute", left: 1460, top: 90 }}>
          <PixelSprite src="game-assets/Sprites/Mars/Relief/aguja-rocosa.png" size={250} />
        </div>

        {/* Craters, Rocks & Mineral Deposits (Real Game Sprites/Mars/Craters, Rocks, Resources) */}
        <div style={{ position: "absolute", left: 460, top: 260 }}>
          <PixelSprite src="game-assets/Sprites/Mars/Craters/crater-grande.png" size={190} />
        </div>
        <div style={{ position: "absolute", left: 1340, top: 290 }}>
          <PixelSprite src="game-assets/Sprites/Mars/Craters/crater-hondo.png" size={175} />
        </div>
        <div style={{ position: "absolute", left: 840, top: 680 }}>
          <PixelSprite src="game-assets/Sprites/Mars/Rocks/roca-ancha.png" size={145} />
        </div>
        <div style={{ position: "absolute", left: 1050, top: 720 }}>
          <PixelSprite src="game-assets/Sprites/Mars/Resources/deposito-cristal.png" size={135} />
        </div>
        <div style={{ position: "absolute", left: 1540, top: 640 }}>
          <PixelSprite src="game-assets/Sprites/Mars/Resources/mena-hierro.png" size={140} />
        </div>

        {/* Real Game Mars Lander Module (Sprites/Mars/Landing/landing_module.png) */}
        <div style={{ position: "absolute", left: 140, top: 470 }}>
          <PixelSprite src="game-assets/Sprites/Mars/Landing/landing_module.png" size={235} />
        </div>

        {/* 3 Real Game Science Points (science_point_delta, science_point_mineral, science_point_special_rock) */}
        <div style={{ position: "absolute", left: 680, top: 450 }}>
          <PixelSprite
            src="game-assets/Sprites/Mars/SciencePoints/science_point_delta.png"
            size={155}
          />
          <div style={{ position: "absolute", left: 48, top: -48 }}>
            <PixelSprite
              src="game-assets/Sprites/Mars/Indicators/science_completed.png"
              size={54}
            />
          </div>
        </div>

        <div style={{ position: "absolute", left: 1160, top: 380 }}>
          <PixelSprite
            src="game-assets/Sprites/Mars/SciencePoints/science_point_special_rock.png"
            size={160}
          />
          {frame >= 65 && (
            <div style={{ position: "absolute", left: -18, top: -18 }}>
              <PixelSprite
                src={`game-assets/Sprites/Mars/Effects/Scan/scan_00${scanIdx}.png`}
                size={195}
              />
            </div>
          )}
        </div>

        <div style={{ position: "absolute", left: 1520, top: 450 }}>
          <PixelSprite
            src="game-assets/Sprites/Mars/SciencePoints/science_point_mineral.png"
            size={155}
          />
          <div style={{ position: "absolute", left: 48, top: -48 }}>
            <PixelSprite
              src={`game-assets/Sprites/Mars/Indicators/Interaction/interaction_e_00${frame % 2}.png`}
              size={52}
            />
          </div>
        </div>

        {/* Real Game Driving Rover (DrivingV2/East/drive_east_000..003.png) */}
        <div style={{ position: "absolute", left: roverX, top: roverY }}>
          <PixelSprite src={roverSprite} size={190} />
        </div>

        {/* Real Game Dust Storm Overlay (mars_dust_texture.png) */}
        {isStorm && (
          <AbsoluteFill
            style={{
              backgroundImage: `url(${staticFile(
                "game-assets/Sprites/Mars/Effects/Dust/mars_dust_texture.png"
              )})`,
              backgroundSize: "512px 512px",
              backgroundPosition: `${-frame * 28}px ${frame * 10}px`,
              opacity: 0.45,
              mixBlendMode: "screen",
            }}
          />
        )}
      </AbsoluteFill>

      <ForegroundParticles
        color="#fb923c"
        secondaryColor="#fde047"
        count={24}
        speedY={-2.5}
        speedX={6.0}
      />

      {/* Top-Left Narrative Stamp: EXPLORA */}
      <div
        style={{
          position: "absolute",
          top: 28,
          left: 40,
          transform: `scale(${stampPop})`,
          padding: "12px 26px",
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
          imageRendering: "pixelated",
          border: "4px solid #f97316",
          boxShadow: "0 14px 36px rgba(0,0,0,0.85)",
          zIndex: 65,
        }}
      >
        <div
          style={{
            fontFamily: FONTS.title,
            fontSize: 52,
            color: "#fb923c",
            letterSpacing: 5,
            lineHeight: 1,
          }}
        >
          EXPLORA
        </div>
        <div
          style={{
            fontFamily: FONTS.body,
            fontSize: 17,
            fontWeight: 900,
            color: "#f8fafc",
            letterSpacing: 1.5,
            marginTop: 4,
          }}
        >
          04_MARS // CRÁTER JEZERO, DELTA FLUVIAL Y TORMENTA DE POLVO
        </div>
      </div>

      {/* Top-Right Real GameMars HUD (Energy, Science Points, Sols, Storm Alert) */}
      <div
        style={{
          position: "absolute",
          top: 28,
          right: 40,
          padding: "12px 22px",
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
          imageRendering: "pixelated",
          border: `3px solid ${isStorm ? "#ef4444" : "#f59e0b"}`,
          display: "flex",
          alignItems: "center",
          gap: 20,
          zIndex: 65,
        }}
      >
        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
          <PixelSprite src="game-assets/Sprites/Icons/Resources/energy.png" size={30} />
          <span style={{ fontFamily: FONTS.title, fontSize: 22, color: "#4ade80" }}>
            RTG 92 %
          </span>
        </div>
        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
          <PixelSprite src="game-assets/Sprites/Icons/Resources/science_data.png" size={30} />
          <span style={{ fontFamily: FONTS.title, fontSize: 22, color: "#38bdf8" }}>
            2 / 3 PUNTOS
          </span>
        </div>
        {isStorm && (
          <div
            style={{
              padding: "4px 12px",
              background: "rgba(239,68,68,0.28)",
              border: "2px solid #ef4444",
              fontFamily: FONTS.title,
              fontSize: 18,
              color: "#fca5a5",
            }}
          >
            ⚠ TORMENTA DE POLVO
          </div>
        )}
      </div>

      {/* Bottom Real Game Science Popup (UI/Science/science_popup_frame.png) */}
      <div
        style={{
          position: "absolute",
          bottom: 28,
          left: 140,
          right: 140,
          padding: "14px 26px",
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
          imageRendering: "pixelated",
          border: "3px solid #f59e0b",
          boxShadow: "0 16px 40px rgba(0,0,0,0.92)",
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          zIndex: 65,
        }}
      >
        <div style={{ display: "flex", alignItems: "center", gap: 16 }}>
          <PixelSprite
            src="game-assets/Sprites/Mars/SciencePoints/science_point_special_rock.png"
            size={64}
          />
          <div>
            <div style={{ fontFamily: FONTS.title, fontSize: 22, color: "#facc15" }}>
              DATO CIENTÍFICO NASA — DELTA FLUVIAL DEL CRÁTER JEZERO
            </div>
            <div style={{ fontFamily: FONTS.body, fontSize: 19, fontWeight: 800, color: "#f8fafc", marginTop: 2 }}>
              Perseverance confirmó que el Cráter Jezero albergó un antiguo lago y un delta hace más de 3.500 millones de años.
            </div>
          </div>
        </div>
      </div>

      <TacticalHudFrame />
    </AbsoluteFill>
  );
};
