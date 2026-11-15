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

export const Scene06_MoonSecret: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  const camScale = interpolate(frame, [0, 150], [1.04, 1.2], {
    extrapolateRight: "clamp",
  });
  const camX = interpolate(frame, [0, 150], [80, -90], {
    extrapolateRight: "clamp",
  });

  const roverX = interpolate(frame, [0, 150], [320, 1060], {
    extrapolateRight: "clamp",
  });
  const roverY = interpolate(frame, [0, 150], [500, 460], {
    extrapolateRight: "clamp",
  });

  const driveIdx = Math.floor(frame / 3) % 4;
  const roverSprite = `game-assets/Sprites/Rover/DrivingV2/East/drive_east_00${driveIdx}.png`;
  const scanIdx = Math.floor(frame / 3) % 4;

  const stampPop = spring({
    frame: frame - 8,
    fps,
    config: { damping: 12, stiffness: 170 },
  });

  return (
    <AbsoluteFill style={{ backgroundColor: "#090d1a", overflow: "hidden" }}>
      <DiagonalWipeIn color="#38bdf8" />
      <ImpactFlash triggerFrame={75} duration={8} color="#38bdf8" maxOpacity={0.65} />

      {/* REAL GAME SCENARIO: 04_Moon Expanded Tilemap (MoonTilesPNG + Rocks + Permanent Shadow Crater + SciencePoints) */}
      <AbsoluteFill
        style={{
          transform: `translate(${camX}px, 0px) scale(${camScale})`,
          transformOrigin: "center center",
        }}
      >
        {/* Base Sunlit Lunar Regolith Tilemap (MoonTilesPNG/dirt_a.png) */}
        <AbsoluteFill
          style={{
            backgroundImage: `url(${staticFile(
              "game-assets/Tilesets/Moon/MoonTilesPNG/dirt_a.png"
            )})`,
            backgroundSize: "96px 96px",
            imageRendering: "pixelated",
          }}
        />

        {/* Permanently Shadowed Polar Crater Zone (-240 °C, crater_dark.png + shadow_soft.png) */}
        <div
          style={{
            position: "absolute",
            left: 680,
            top: 120,
            width: 1120,
            height: 760,
            borderRadius: "48%",
            background: "radial-gradient(circle, rgba(6,12,30,0.88) 0%, rgba(4,8,22,0.95) 80%)",
            imageRendering: "pixelated",
            border: "6px dashed rgba(56,189,248,0.65)",
            boxShadow: "inset 0 0 120px rgba(2,6,23,0.92)",
          }}
        />

        {/* Real Game Lunar Lander (landing_module_moon.png) */}
        <div style={{ position: "absolute", left: 130, top: 430 }}>
          <PixelSprite
            src="game-assets/Sprites/Moon/Landing/landing_module_moon.png"
            size={230}
          />
        </div>

        {/* Real Game Lunar Rocks & Spikes Around the Crater Rim */}
        <div style={{ position: "absolute", left: 520, top: 180 }}>
          <PixelSprite src="game-assets/Sprites/Moon/Rocks/rock_big_1.png" size={165} />
        </div>
        <div style={{ position: "absolute", left: 640, top: 690 }}>
          <PixelSprite src="game-assets/Sprites/Moon/Rocks/rock_big_3.png" size={160} />
        </div>
        <div style={{ position: "absolute", left: 960, top: 160 }}>
          <PixelSprite src="game-assets/Sprites/Moon/Rocks/rock_spike_1.png" size={150} />
        </div>
        <div style={{ position: "absolute", left: 1480, top: 240 }}>
          <PixelSprite src="game-assets/Sprites/Moon/Rocks/rock_spike_2.png" size={155} />
        </div>
        <div style={{ position: "absolute", left: 1420, top: 660 }}>
          <PixelSprite src="game-assets/Sprites/Moon/Rocks/rock_big_2.png" size={165} />
        </div>

        {/* 3 Real Game Lunar Science Points (science_point_regolith, science_point_rim_rock, science_point_ice) */}
        <div style={{ position: "absolute", left: 480, top: 460 }}>
          <PixelSprite
            src="game-assets/Sprites/Moon/SciencePoints/science_point_regolith.png"
            size={150}
          />
          <div style={{ position: "absolute", left: 45, top: -48 }}>
            <PixelSprite
              src="game-assets/Sprites/Mars/Indicators/science_completed.png"
              size={52}
            />
          </div>
        </div>

        <div style={{ position: "absolute", left: 860, top: 310 }}>
          <PixelSprite
            src="game-assets/Sprites/Moon/SciencePoints/science_point_rim_rock.png"
            size={150}
          />
          <div style={{ position: "absolute", left: 45, top: -48 }}>
            <PixelSprite
              src="game-assets/Sprites/Mars/Indicators/science_completed.png"
              size={52}
            />
          </div>
        </div>

        <div style={{ position: "absolute", left: 1180, top: 430 }}>
          <PixelSprite
            src="game-assets/Sprites/Moon/SciencePoints/science_point_ice.png"
            size={170}
          />
          {frame >= 55 && (
            <div style={{ position: "absolute", left: -15, top: -15 }}>
              <PixelSprite
                src={`game-assets/Sprites/Mars/Effects/Scan/scan_00${scanIdx}.png`}
                size={200}
              />
            </div>
          )}
        </div>

        {/* Rover Driving into the Shadowed Crater with Dual Cyan Headlights */}
        <div style={{ position: "absolute", left: roverX, top: roverY }}>
          <div
            style={{
              position: "absolute",
              left: 135,
              top: 5,
              width: 340,
              height: 185,
              background:
                "linear-gradient(90deg, rgba(56,189,248,0.65) 0%, rgba(56,189,248,0.18) 65%, transparent 100%)",
              clipPath: "polygon(0% 38%, 100% 0%, 100% 100%, 0% 62%)",
              mixBlendMode: "screen",
            }}
          />
          <PixelSprite src={roverSprite} size={190} />
        </div>
      </AbsoluteFill>

      {/* Picture-in-Picture High-Impact Action Cut: Percussion Drill & Water-Ice Geyser! */}
      {frame >= 65 && (
        <div
          style={{
            position: "absolute",
            right: 40,
            top: 135,
            width: 490,
            height: 280,
            padding: 8,
            background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
            border: "4px solid #38bdf8",
            boxShadow: "0 18px 45px rgba(0,0,0,0.92), 0 0 30px rgba(56,189,248,0.55)",
            zIndex: 65,
          }}
        >
          <Img
            src={staticFile("cinematic/moon_lava_tube_ice_drill.png")}
            style={{
              width: "100%",
              height: "100%",
              objectFit: "cover",
              imageRendering: "pixelated",
            }}
          />
          <div
            style={{
              position: "absolute",
              bottom: 14,
              left: 14,
              right: 14,
              background: "rgba(2,6,23,0.85)",
              padding: "6px 12px",
              border: "2px solid #38bdf8",
              fontFamily: FONTS.title,
              fontSize: 17,
              color: "#38bdf8",
              textAlign: "center",
            }}
          >
            ¡TALADRO PERCUTOR: HIELO DE AGUA (H₂O) DETECTADO!
          </div>
        </div>
      )}

      <ForegroundParticles
        color="#38bdf8"
        secondaryColor="#e2e8f0"
        count={26}
        speedY={-2.5}
        speedX={2.0}
      />

      {/* Top-Left Narrative Stamp: DESCUBRE */}
      <div
        style={{
          position: "absolute",
          top: 28,
          left: 40,
          transform: `scale(${stampPop})`,
          padding: "12px 26px",
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
          imageRendering: "pixelated",
          border: "4px solid #38bdf8",
          boxShadow: "0 14px 36px rgba(0,0,0,0.85)",
          zIndex: 65,
        }}
      >
        <div
          style={{
            fontFamily: FONTS.title,
            fontSize: 52,
            color: "#38bdf8",
            letterSpacing: 5,
            lineHeight: 1,
          }}
        >
          DESCUBRE
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
          04_MOON // SOMBRA PERMANENTE (-240 °C) Y HIELO POLAR
        </div>
      </div>

      {/* Top-Right Lunar HUD */}
      <div
        style={{
          position: "absolute",
          top: 28,
          right: 40,
          padding: "12px 22px",
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
          imageRendering: "pixelated",
          border: "3px solid #38bdf8",
          display: "flex",
          alignItems: "center",
          gap: 18,
          zIndex: 65,
        }}
      >
        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
          <PixelSprite src="game-assets/Sprites/Icons/Resources/communications.png" size={28} />
          <span style={{ fontFamily: FONTS.title, fontSize: 20, color: "#38bdf8" }}>
            RELÉ ORBITAL ACTIVO
          </span>
        </div>
        <div
          style={{
            padding: "4px 12px",
            background: "rgba(56,189,248,0.22)",
            border: "2px solid #38bdf8",
            fontFamily: FONTS.title,
            fontSize: 18,
            color: "#7dd3fc",
          }}
        >
          ❄ FRÍO EXTREMO -2.0 E/s
        </div>
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
          border: "3px solid #38bdf8",
          boxShadow: "0 16px 40px rgba(0,0,0,0.92)",
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          zIndex: 65,
        }}
      >
        <div style={{ display: "flex", alignItems: "center", gap: 16 }}>
          <PixelSprite
            src="game-assets/Sprites/Moon/SciencePoints/science_point_ice.png"
            size={64}
          />
          <div>
            <div style={{ fontFamily: FONTS.title, fontSize: 22, color: "#38bdf8" }}>
              HALLAZGO LUNAR — DEPÓSITOS DE HIELO EN CRÁTER CABEUS (NASA LRO / LCROSS / VIPER)
            </div>
            <div style={{ fontFamily: FONTS.body, fontSize: 19, fontWeight: 800, color: "#f8fafc", marginTop: 2 }}>
              En las regiones polares en sombra permanente se conserva hielo de agua clave para futuras bases espaciales.
            </div>
          </div>
        </div>
      </div>

      <TacticalHudFrame />
    </AbsoluteFill>
  );
};
