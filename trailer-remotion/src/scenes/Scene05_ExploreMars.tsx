import React from "react";
import {
  AbsoluteFill,
  interpolate,
  spring,
  staticFile,
  useCurrentFrame,
  useVideoConfig,
} from "remotion";
import { PixelSprite } from "../components/PixelSprite";
import { GroundedSprite, MarsTilemap } from "../components/GameTilemap";
import { FONTS } from "../styles/theme";
import {
  DiagonalWipeIn,
  ImpactFlash,
  TacticalHudFrame,
} from "../components/CinematicEffects";

export const Scene05_ExploreMars: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  const camScale = interpolate(frame, [0, 180], [1.04, 1.16], {
    extrapolateRight: "clamp",
  });
  const camX = interpolate(frame, [0, 180], [70, -85], {
    extrapolateRight: "clamp",
  });
  const camY = interpolate(frame, [0, 180], [15, -20], {
    extrapolateRight: "clamp",
  });

  // Rover drives along the unobstructed sandy river channel (below science_point_delta at y=420)
  // and stops cleanly in front of science_point_special_rock (x=1140, y=425) at x=985, y=465 to scan it without clipping!
  const isDriving = frame < 115;
  const roverX = interpolate(frame, [0, 115], [410, 985], {
    extrapolateRight: "clamp",
  });
  const roverY = interpolate(frame, [0, 60, 115], [565, 515, 465], {
    extrapolateRight: "clamp",
  });

  const driveIdx = isDriving ? Math.floor(frame / 3) % 4 : 0;
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
      <ImpactFlash triggerFrame={105} duration={7} color="#f97316" maxOpacity={0.5} />

      <AbsoluteFill
        style={{
          transform: `translate(${camX}px, ${camY}px) scale(${camScale})`,
          transformOrigin: "center center",
        }}
      >
        <MarsTilemap />

        {/* Grounded Cliffs, Mesas, Craters, Rocks & Minerals with Contact Shadows */}
        <GroundedSprite
          src="game-assets/Sprites/Mars/Relief/meseta-grande.png"
          x={190}
          y={180}
          size={260}
        />
        <GroundedSprite
          src="game-assets/Sprites/Mars/Relief/cantil-largo.png"
          x={580}
          y={165}
          size={250}
        />
        <GroundedSprite
          src="game-assets/Sprites/Mars/Relief/meseta-alta.png"
          x={1040}
          y={170}
          size={255}
        />
        <GroundedSprite
          src="game-assets/Sprites/Mars/Relief/aguja-rocosa.png"
          x={1520}
          y={195}
          size={235}
        />

        <GroundedSprite
          src="game-assets/Sprites/Mars/Craters/crater-grande.png"
          x={490}
          y={315}
          size={175}
          shadowHeight={20}
        />
        <GroundedSprite
          src="game-assets/Sprites/Mars/Craters/crater-hondo.png"
          x={1380}
          y={315}
          size={165}
          shadowHeight={20}
        />

        <GroundedSprite
          src="game-assets/Sprites/Mars/Rocks/roca-ancha.png"
          x={840}
          y={740}
          size={140}
        />
        <GroundedSprite
          src="game-assets/Sprites/Mars/Resources/deposito-cristal.png"
          x={1060}
          y={760}
          size={130}
        />
        <GroundedSprite
          src="game-assets/Sprites/Mars/Resources/mena-hierro.png"
          x={1540}
          y={700}
          size={135}
        />

        {/* Grounded Mars Lander Module */}
        <GroundedSprite
          src="game-assets/Sprites/Mars/Landing/landing_module.png"
          x={210}
          y={565}
          size={220}
          shadowWidth={160}
        />

        {/* 3 Grounded Science Points Positioned Along the Channel Bank so the Rover Scans Without Clipping */}
        <GroundedSprite
          src="game-assets/Sprites/Mars/SciencePoints/science_point_delta.png"
          x={680}
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
          src="game-assets/Sprites/Mars/SciencePoints/science_point_special_rock.png"
          x={1145}
          y={435}
          size={145}
        >
          {frame >= 65 && (
            <div style={{ position: "absolute", left: -20, top: -20 }}>
              <PixelSprite
                src={`game-assets/Sprites/Mars/Effects/Scan/scan_00${scanIdx}.png`}
                size={185}
              />
            </div>
          )}
        </GroundedSprite>

        <GroundedSprite
          src="game-assets/Sprites/Mars/SciencePoints/science_point_mineral.png"
          x={1520}
          y={490}
          size={145}
        >
          <div style={{ position: "absolute", left: 48, top: -44 }}>
            <PixelSprite
              src={`game-assets/Sprites/Mars/Indicators/Interaction/interaction_e_00${frame % 2}.png`}
              size={48}
            />
          </div>
        </GroundedSprite>

        {/* Grounded Driving Rover with Wheel Contact Shadow & Y-Sorting */}
        <GroundedSprite
          src={roverSprite}
          x={roverX}
          y={roverY}
          size={185}
          shadowWidth={140}
          shadowHeight={38}
        />

        {/* Real Game Dust Storm Overlay */}
        {isStorm && (
          <AbsoluteFill
            style={{
              backgroundImage: `url(${staticFile(
                "game-assets/Sprites/Mars/Effects/Dust/mars_dust_texture.png"
              )})`,
              backgroundSize: "512px 512px",
              backgroundPosition: `${-frame * 26}px ${frame * 9}px`,
              opacity: 0.42,
              mixBlendMode: "screen",
              zIndex: 2000,
            }}
          />
        )}
      </AbsoluteFill>

      {/* Top-Left Narrative Stamp: EXPLORA */}
      <div
        style={{
          position: "absolute",
          top: 28,
          left: 40,
          transform: `scale(${stampPop})`,
          padding: "12px 26px",
          background:
            "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
          border: "4px solid #f97316",
          boxShadow: "0 14px 36px rgba(0,0,0,0.85)",
          zIndex: 90,
        }}
      >
        <div
          style={{
            fontFamily: FONTS.title,
            fontSize: 50,
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
          04_MARS // CRÁTER JEZERO Y ANTIGUO DELTA FLUVIAL
        </div>
      </div>

      {/* Top-Right Mars HUD */}
      <div
        style={{
          position: "absolute",
          top: 28,
          right: 40,
          padding: "12px 22px",
          background:
            "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
          border: `3px solid ${isStorm ? "#ef4444" : "#f59e0b"}`,
          display: "flex",
          alignItems: "center",
          gap: 18,
          zIndex: 90,
        }}
      >
        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
          <PixelSprite
            src="game-assets/Sprites/Icons/Resources/energy.png"
            size={28}
          />
          <span
            style={{
              fontFamily: FONTS.title,
              fontSize: 21,
              color: "#4ade80",
            }}
          >
            RTG 92 %
          </span>
        </div>
        <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
          <PixelSprite
            src="game-assets/Sprites/Icons/Resources/science_data.png"
            size={28}
          />
          <span
            style={{
              fontFamily: FONTS.title,
              fontSize: 21,
              color: "#38bdf8",
            }}
          >
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
          border: "3px solid #f59e0b",
          boxShadow: "0 16px 40px rgba(0,0,0,0.92)",
          display: "flex",
          alignItems: "center",
          gap: 16,
          zIndex: 90,
        }}
      >
        <PixelSprite
          src="game-assets/Sprites/Mars/SciencePoints/science_point_special_rock.png"
          size={60}
        />
        <div>
          <div
            style={{
              fontFamily: FONTS.title,
              fontSize: 22,
              color: "#facc15",
            }}
          >
            DATO CIENTÍFICO NASA — DELTA DEL CRÁTER JEZERO
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
            Perseverance confirmó que el Cráter Jezero albergó un antiguo lago y un delta fluvial hace más de 3.500 millones de años.
          </div>
        </div>
      </div>

      <TacticalHudFrame />
    </AbsoluteFill>
  );
};
