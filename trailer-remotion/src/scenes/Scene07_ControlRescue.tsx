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
  SpeedLinesOverlay,
  TacticalHudFrame,
} from "../components/CinematicEffects";

export const Scene07_ControlRescue: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  // Shot 1 (0..92): The dramatic Rover Sandstorm Crisis with sparks flying (rover_sandstorm_crisis.png — loved by user!)
  // Shot 2 (92..210): Split-screen REAL GAME SCENARIOS: Left = 01_Hub Control Room with Luna, Mateo & Vega + DSN 70m Dish; Right = 04_Mars Tilemap Rover rescued!
  const isRescue = frame >= 92;

  const shakeX = !isRescue ? Math.sin(frame * 2.2) * 10 : 0;
  const shakeY = !isRescue ? Math.cos(frame * 2.6) * 10 : 0;

  const stampPop = spring({
    frame: frame - 8,
    fps,
    config: { damping: 12, stiffness: 170 },
  });

  const driveIdx = Math.floor(frame / 3) % 4;
  const scanIdx = Math.floor(frame / 3) % 4;

  return (
    <AbsoluteFill style={{ backgroundColor: "#090408", overflow: "hidden" }}>
      <DiagonalWipeIn color="#ef4444" />
      <ImpactFlash triggerFrame={0} duration={10} color="#ef4444" maxOpacity={0.8} />
      <ImpactFlash triggerFrame={92} duration={10} color="#38bdf8" maxOpacity={0.85} />

      {/* SHOT 1: ROVER SANDSTORM CRISIS & SPARKS */}
      {!isRescue && (
        <AbsoluteFill
          style={{
            transform: `translate(${shakeX}px, ${shakeY}px) scale(${interpolate(
              frame,
              [0, 92],
              [1.06, 1.24]
            )})`,
          }}
        >
          <Img
            src={staticFile("cinematic/rover_sandstorm_crisis.png")}
            style={{
              width: "100%",
              height: "100%",
              objectFit: "cover",
              imageRendering: "pixelated",
            }}
          />
          <SpeedLinesOverlay opacity={0.34} color="#ef4444" direction="diagonal" />
          <ForegroundParticles
            color="#facc15"
            secondaryColor="#ef4444"
            count={38}
            speedY={-8}
            speedX={7}
          />
        </AbsoluteFill>
      )}

      {/* SHOT 2: REAL GAME SCENARIO SPLIT-SCREEN RESCUE (01_Hub Control + DSN Dish <-> 04_Mars Rover Rescued!) */}
      {isRescue && (
        <AbsoluteFill style={{ display: "flex", flexDirection: "row" }}>
          {/* LEFT HALF: Real 01_Hub Mission Control Scenario + DSN Dish Feed */}
          <div
            style={{
              width: "50%",
              height: "100%",
              position: "relative",
              overflow: "hidden",
              borderRight: "6px solid #38bdf8",
            }}
          >
            <AbsoluteFill
              style={{
                backgroundImage: `url(${staticFile(
                  "game-assets/Tilesets/Hub/LabTilesPNG/floor_panels.png"
                )})`,
                backgroundSize: "96px 96px",
                imageRendering: "pixelated",
              }}
            />
            <div style={{ position: "absolute", left: 80, top: 180 }}>
              <PixelSprite src="game-assets/Sprites/Hub/Interior/consola-control.png" size={220} />
            </div>
            <div style={{ position: "absolute", left: 340, top: 180 }}>
              <PixelSprite src="game-assets/Sprites/Hub/Interior/estacion-diagnostico.png" size={220} />
            </div>
            <div style={{ position: "absolute", left: 620, top: 190 }}>
              <PixelSprite src="game-assets/Sprites/Hub/Interior/reactor-cian.png" size={210} />
            </div>

            {/* Luna Torres, Mateo Rojas & Dra. Vega executing the recovery protocol */}
            <div style={{ position: "absolute", left: 150, top: 360 }}>
              <PixelSprite src="game-assets/Sprites/Characters/Luna/Idle/South/idle_south_000.png" size={145} />
            </div>
            <div style={{ position: "absolute", left: 390, top: 360 }}>
              <PixelSprite src="game-assets/Sprites/Characters/Mateo/Idle/South/idle_south_000.png" size={145} />
            </div>
            <div style={{ position: "absolute", left: 620, top: 360 }}>
              <PixelSprite src="game-assets/Sprites/Characters/DrElenaVega/Idle/South/vega-idle-south_000.png" size={145} />
            </div>

            {/* Inset Live Feed of the 70m Deep Space Network Dish */}
            <div
              style={{
                position: "absolute",
                left: 60,
                bottom: 205,
                width: 440,
                height: 230,
                padding: 6,
                background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
                border: "3px solid #38bdf8",
              }}
            >
              <Img
                src={staticFile("cinematic/dsn_mars_yard_rescue.png")}
                style={{
                  width: "100%",
                  height: "100%",
                  objectFit: "cover",
                  imageRendering: "pixelated",
                }}
              />
            </div>
          </div>

          {/* RIGHT HALF: Real 04_Mars Game Scenario — Rover Rebooted in Safe Mode & Completing Scan! */}
          <div
            style={{
              width: "50%",
              height: "100%",
              position: "relative",
              overflow: "hidden",
            }}
          >
            <AbsoluteFill
              style={{
                backgroundImage: `url(${staticFile(
                  "game-assets/Tilesets/Mars/MarsTilesPNG/sand_a.png"
                )})`,
                backgroundSize: "96px 96px",
                imageRendering: "pixelated",
              }}
            />
            <div style={{ position: "absolute", left: 120, top: 170 }}>
              <PixelSprite src="game-assets/Sprites/Mars/Relief/meseta-alta.png" size={230} />
            </div>
            <div style={{ position: "absolute", left: 560, top: 200 }}>
              <PixelSprite src="game-assets/Sprites/Mars/Rocks/roca-alta.png" size={160} />
            </div>

            {/* Science Point Being Completed */}
            <div style={{ position: "absolute", left: 540, top: 430 }}>
              <PixelSprite
                src="game-assets/Sprites/Mars/SciencePoints/science_point_delta.png"
                size={165}
              />
              <div style={{ position: "absolute", left: -15, top: -15 }}>
                <PixelSprite
                  src={`game-assets/Sprites/Mars/Effects/Scan/scan_00${scanIdx}.png`}
                  size={195}
                />
              </div>
              <div style={{ position: "absolute", left: 52, top: -52 }}>
                <PixelSprite
                  src="game-assets/Sprites/Mars/Indicators/science_completed.png"
                  size={58}
                />
              </div>
            </div>

            {/* Rescued Rover with Glowing Green Safe-Mode Recovery Shield */}
            <div
              style={{
                position: "absolute",
                left: interpolate(frame, [92, 210], [180, 390], { extrapolateRight: "clamp" }),
                top: 440,
              }}
            >
              <div
                style={{
                  position: "absolute",
                  left: -20,
                  top: -20,
                  width: 240,
                  height: 240,
                  borderRadius: "50%",
                  border: "4px solid #4ade80",
                  background: "radial-gradient(circle, rgba(74,222,128,0.35) 0%, transparent 70%)",
                }}
              />
              <PixelSprite
                src={`game-assets/Sprites/Rover/DrivingV2/East/drive_east_00${driveIdx}.png`}
                size={195}
              />
            </div>
          </div>
        </AbsoluteFill>
      )}

      {/* Top-Right Narrative Stamp: RESUELVE */}
      <div
        style={{
          position: "absolute",
          top: 28,
          right: 40,
          transform: `scale(${stampPop})`,
          padding: "12px 26px",
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
          imageRendering: "pixelated",
          border: `4px solid ${!isRescue ? "#ef4444" : "#22c55e"}`,
          boxShadow: "0 14px 36px rgba(0,0,0,0.85)",
          textAlign: "right",
          zIndex: 65,
        }}
      >
        <div
          style={{
            fontFamily: FONTS.title,
            fontSize: 52,
            color: !isRescue ? "#f87171" : "#4ade80",
            letterSpacing: 5,
            lineHeight: 1,
          }}
        >
          RESUELVE
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
          {!isRescue
            ? "¡CRISIS EN MARTE: TORMENTA Y APAGÓN!"
            : "01_HUB ↔ 04_MARS // ¡RESCATE EN MODO SEGURO!"}
        </div>
      </div>

      {/* Top-Left Status Badge */}
      <div
        style={{
          position: "absolute",
          top: 28,
          left: 40,
          padding: "12px 22px",
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
          imageRendering: "pixelated",
          border: `3px solid ${!isRescue ? "#ef4444" : "#22c55e"}`,
          display: "flex",
          alignItems: "center",
          gap: 12,
          zIndex: 65,
        }}
      >
        <PixelSprite
          src={
            !isRescue
              ? "game-assets/Sprites/Icons/Resources/no_signal.png"
              : "game-assets/Sprites/Icons/Resources/safe_mode.png"
          }
          size={38}
        />
        <span
          style={{
            fontFamily: FONTS.title,
            fontSize: 22,
            color: !isRescue ? "#f87171" : "#4ade80",
          }}
        >
          {!isRescue
            ? "¡ALERTA: SIN SEÑAL Y PANELES A 0 E/s!"
            : "ENLACE DSN 70m + RESERVA DE MODO SEGURO (+0.8 E/s)"}
        </span>
      </div>

      {/* Bottom Real Game Dialogue Box (UI/Dialogue/dialogue_frame.png) */}
      <div
        style={{
          position: "absolute",
          bottom: 28,
          left: 120,
          right: 120,
          height: 152,
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
          imageRendering: "pixelated",
          border: `3px solid ${!isRescue ? "#ef4444" : "#22c55e"}`,
          boxShadow: "0 18px 45px rgba(0,0,0,0.92)",
          display: "flex",
          alignItems: "center",
          padding: "16px 28px",
          gap: 22,
          zIndex: 65,
        }}
      >
        <div
          style={{
            width: 110,
            height: 110,
            background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            flexShrink: 0,
          }}
        >
          <PixelSprite
            src={
              !isRescue
                ? "game-assets/Sprites/Portraits/BIMO/bimo_alert.png"
                : "game-assets/Sprites/Portraits/Luna/luna_neutral.png"
            }
            size={92}
          />
        </div>
        <div>
          <div
            style={{
              fontFamily: FONTS.title,
              fontSize: 23,
              color: !isRescue ? "#f87171" : "#4ade80",
              marginBottom: 4,
            }}
          >
            {!isRescue
              ? "BIMO — ¡ROVER AVERIADO EN LA TORMENTA DE ARENA!"
              : "LUNA TORRES & MATEO ROJAS — RESCATE DESDE EL CENTRO DE CONTROL"}
          </div>
          <div
            style={{
              fontFamily: FONTS.body,
              fontSize: 22,
              fontWeight: 800,
              color: "#f8fafc",
              lineHeight: 1.3,
            }}
          >
            {!isRescue
              ? "«¡Comandante! El polvo bloqueó los paneles y saltan chispas de la antena. ¡Activando Modo Seguro!»"
              : "«¡Red de Espacio Profundo conectada! El rover recupera energía con su reserva y completa el escaneo 3/3.»"}
          </div>
        </div>
      </div>

      <TacticalHudFrame alertMode={!isRescue} />
    </AbsoluteFill>
  );
};
