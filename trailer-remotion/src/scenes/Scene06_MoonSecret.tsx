import React from "react";
import {
  AbsoluteFill,
  interpolate,
  spring,
  useCurrentFrame,
  useVideoConfig,
} from "remotion";
import { PixelSprite } from "../components/PixelSprite";
import { GroundedSprite, MoonTilemap } from "../components/GameTilemap";
import { FONTS } from "../styles/theme";
import {
  DiagonalWipeIn,
  ImpactFlash,
  TacticalHudFrame,
} from "../components/CinematicEffects";

export const Scene06_MoonSecret: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  const camScale = interpolate(frame, [0, 150], [1.04, 1.16], {
    extrapolateRight: "clamp",
  });
  const camX = interpolate(frame, [0, 150], [70, -85], {
    extrapolateRight: "clamp",
  });

  // Rover drives from the sunlit lander ridge into the -240 C Cabeus shadow basin
  // and stops cleanly at (1055, 500) so its headlight beam illuminates science_point_ice at (1285, 490) without overlapping!
  const isDriving = frame < 105;
  const roverX = interpolate(frame, [0, 105], [405, 1055], {
    extrapolateRight: "clamp",
  });
  const roverY = interpolate(frame, [0, 105], [555, 500], {
    extrapolateRight: "clamp",
  });

  const driveIdx = isDriving ? Math.floor(frame / 3) % 4 : 0;
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
      <ImpactFlash triggerFrame={70} duration={7} color="#38bdf8" maxOpacity={0.5} />

      <AbsoluteFill
        style={{
          transform: `translate(${camX}px, 0px) scale(${camScale})`,
          transformOrigin: "center center",
        }}
      >
        <MoonTilemap />

        {/* Grounded Lunar Lander on the Sunlit Ridge */}
        <GroundedSprite
          src="game-assets/Sprites/Moon/Landing/landing_module_moon.png"
          x={210}
          y={530}
          size={220}
          shadowWidth={160}
        />

        {/* Grounded Lunar Rocks & Spikes Around the Shadow Basin Rim */}
        <GroundedSprite
          src="game-assets/Sprites/Moon/Rocks/rock_big_1.png"
          x={620}
          y={245}
          size={160}
        />
        <GroundedSprite
          src="game-assets/Sprites/Moon/Rocks/rock_big_3.png"
          x={740}
          y={750}
          size={155}
        />
        <GroundedSprite
          src="game-assets/Sprites/Moon/Rocks/rock_spike_1.png"
          x={1040}
          y={225}
          size={145}
        />
        <GroundedSprite
          src="game-assets/Sprites/Moon/Rocks/rock_spike_2.png"
          x={1520}
          y={300}
          size={150}
        />
        <GroundedSprite
          src="game-assets/Sprites/Moon/Rocks/rock_big_2.png"
          x={1460}
          y={720}
          size={160}
        />

        {/* 3 Grounded Lunar Science Points (Positioned Clear of the Rover's Driving Path!) */}
        <GroundedSprite
          src="game-assets/Sprites/Moon/SciencePoints/science_point_regolith.png"
          x={560}
          y={415}
          size={140}
        >
          <div style={{ position: "absolute", left: 45, top: -42 }}>
            <PixelSprite
              src="game-assets/Sprites/Mars/Indicators/science_completed.png"
              size={50}
            />
          </div>
        </GroundedSprite>

        <GroundedSprite
          src="game-assets/Sprites/Moon/SciencePoints/science_point_rim_rock.png"
          x={885}
          y={360}
          size={145}
        >
          <div style={{ position: "absolute", left: 46, top: -42 }}>
            <PixelSprite
              src="game-assets/Sprites/Mars/Indicators/science_completed.png"
              size={50}
            />
          </div>
        </GroundedSprite>

        <GroundedSprite
          src="game-assets/Sprites/Moon/SciencePoints/science_point_ice.png"
          x={1285}
          y={490}
          size={160}
        >
          {frame >= 55 && (
            <div style={{ position: "absolute", left: -18, top: -18 }}>
              <PixelSprite
                src={`game-assets/Sprites/Mars/Effects/Scan/scan_00${scanIdx}.png`}
                size={196}
              />
            </div>
          )}
        </GroundedSprite>

        {/* Grounded Rover Driving into the Shadow Basin with Headlight Beam Aimed at the Ice Deposit */}
        <GroundedSprite
          src={roverSprite}
          x={roverX}
          y={roverY}
          size={185}
          shadowWidth={140}
          shadowHeight={38}
        >
          <div
            style={{
              position: "absolute",
              left: 140,
              top: 6,
              width: 310,
              height: 170,
              background:
                "linear-gradient(90deg, rgba(56,189,248,0.58) 0%, rgba(56,189,248,0.16) 68%, transparent 100%)",
              clipPath: "polygon(0% 38%, 100% 0%, 100% 100%, 0% 62%)",
              mixBlendMode: "screen",
            }}
          />
        </GroundedSprite>
      </AbsoluteFill>

      {/* Top-Left Narrative Stamp: DESCUBRE */}
      <div
        style={{
          position: "absolute",
          top: 28,
          left: 40,
          transform: `scale(${stampPop})`,
          padding: "12px 26px",
          background:
            "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
          border: "4px solid #38bdf8",
          boxShadow: "0 14px 36px rgba(0,0,0,0.85)",
          zIndex: 90,
        }}
      >
        <div
          style={{
            fontFamily: FONTS.title,
            fontSize: 50,
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
          04_MOON // CRÁTER EN SOMBRA PERMANENTE (-240 °C)
        </div>
      </div>

      {/* Top-Right Lunar HUD */}
      <div
        style={{
          position: "absolute",
          top: 28,
          right: 40,
          padding: "12px 22px",
          background:
            "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
          border: "3px solid #38bdf8",
          display: "flex",
          alignItems: "center",
          gap: 18,
          zIndex: 90,
        }}
      >
        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
          <PixelSprite
            src="game-assets/Sprites/Icons/Resources/communications.png"
            size={28}
          />
          <span
            style={{
              fontFamily: FONTS.title,
              fontSize: 20,
              color: "#38bdf8",
            }}
          >
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

      {/* Bottom Science Popup */}
      <div
        style={{
          position: "absolute",
          bottom: 28,
          left: 140,
          right: 140,
          padding: "14px 26px",
          background:
            "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
          border: "3px solid #38bdf8",
          boxShadow: "0 16px 40px rgba(0,0,0,0.92)",
          display: "flex",
          alignItems: "center",
          gap: 16,
          zIndex: 90,
        }}
      >
        <PixelSprite
          src="game-assets/Sprites/Moon/SciencePoints/science_point_ice.png"
          size={60}
        />
        <div>
          <div
            style={{
              fontFamily: FONTS.title,
              fontSize: 22,
              color: "#38bdf8",
            }}
          >
            HALLAZGO LUNAR — HIELO DE AGUA EN SOMBRA PERMANENTE (NASA LRO / LCROSS / VIPER)
          </div>
          <div
            style={{
              fontFamily: FONTS.body,
              fontSize: 19,
              fontWeight: 800,
              color: "#f8fafc",
              marginTop: 2,
            }}
          >
            El taladro y el espectrómetro confirman depósitos de hielo polar conservados a -240 °C sin luz solar.
          </div>
        </div>
      </div>

      <TacticalHudFrame />
    </AbsoluteFill>
  );
};
