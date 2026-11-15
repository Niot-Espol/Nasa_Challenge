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
import { GroundedSprite, HubTilemap, MarsTilemap } from "../components/GameTilemap";
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

  const isRescue = frame >= 92;
  const shakeX = !isRescue ? Math.sin(frame * 2.2) * 10 : 0;
  const shakeY = !isRescue ? Math.cos(frame * 2.6) * 10 : 0;

  const stampPop = spring({
    frame: frame - 8,
    fps,
    config: { damping: 12, stiffness: 170 },
  });

  const isRoverMoving = frame < 165;
  const driveIdx = isRoverMoving ? Math.floor(frame / 3) % 4 : 0;
  const scanIdx = Math.floor(frame / 3) % 4;

  return (
    <AbsoluteFill style={{ backgroundColor: "#090408", overflow: "hidden" }}>
      <DiagonalWipeIn color="#ef4444" />
      <ImpactFlash triggerFrame={0} duration={10} color="#ef4444" maxOpacity={0.75} />
      <ImpactFlash triggerFrame={92} duration={10} color="#38bdf8" maxOpacity={0.8} />

      {/* SHOT 1: DRAMATIC ROVER SANDSTORM CRISIS & SPARKS */}
      {!isRescue && (
        <AbsoluteFill
          style={{
            transform: `translate(${shakeX}px, ${shakeY}px) scale(${interpolate(
              frame,
              [0, 92],
              [1.06, 1.22]
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
          <SpeedLinesOverlay opacity={0.32} color="#ef4444" direction="diagonal" />
          <ForegroundParticles
            color="#facc15"
            secondaryColor="#ef4444"
            count={36}
            speedY={-8}
            speedX={7}
          />
        </AbsoluteFill>
      )}

      {/* SHOT 2: CLEAN SPLIT-SCREEN REAL GAME SCENARIOS (01_Hub Control Room <-> 04_Mars Rover Rescued!) */}
      {isRescue && (
        <AbsoluteFill style={{ display: "flex", flexDirection: "row" }}>
          {/* LEFT HALF: Cohesive 01_Hub Mission Control Tilemap + Top Structural Wall + Grounded Crew */}
          <div
            style={{
              width: "50%",
              height: "100%",
              position: "relative",
              overflow: "hidden",
              borderRight: "6px solid #38bdf8",
            }}
          >
            <HubTilemap variant="hub" />
            <div
              style={{
                position: "absolute",
                top: 0,
                left: 0,
                right: 0,
                height: 195,
                background: "linear-gradient(180deg, #0b1120 0%, #1e293b 100%)",
                borderBottom: "8px solid #334155",
                display: "flex",
                alignItems: "flex-end",
                justifyContent: "space-around",
                padding: "0 30px",
                zIndex: 10,
              }}
            >
              <PixelSprite src="game-assets/Sprites/Hub/Structure/muro-panel-a.png" size={160} />
              <PixelSprite src="game-assets/Sprites/Hub/Structure/esclusa-cian.png" size={185} />
              <PixelSprite src="game-assets/Sprites/Hub/Structure/muro-panel-b.png" size={160} />
            </div>

            <GroundedSprite
              src="game-assets/Sprites/Hub/Interior/consola-control.png"
              x={220}
              y={315}
              size={195}
            />
            <GroundedSprite
              src="game-assets/Sprites/Hub/Interior/estacion-diagnostico.png"
              x={480}
              y={315}
              size={195}
            />
            <GroundedSprite
              src="game-assets/Sprites/Hub/Interior/reactor-cian.png"
              x={740}
              y={310}
              size={200}
            />

            <GroundedSprite
              src={`game-assets/Sprites/Characters/Luna/Idle/South/idle_south_00${Math.floor(frame / 6) % 2}.png`}
              x={220}
              y={475}
              size={136}
              shadowWidth={64}
            />
            <GroundedSprite
              src={`game-assets/Sprites/Characters/Mateo/Idle/South/idle_south_00${Math.floor(frame / 6) % 2}.png`}
              x={480}
              y={475}
              size={136}
              shadowWidth={64}
            />
            <GroundedSprite
              src={`game-assets/Sprites/Characters/DrElenaVega/Idle/South/vega-idle-south_00${Math.floor(frame / 4) % 9}.png`}
              x={740}
              y={475}
              size={138}
              shadowWidth={64}
            />
          </div>

          {/* RIGHT HALF: Cohesive 04_Mars Tilemap — Rover Rescued in Safe Mode Completing 3/3 Scan */}
          <div
            style={{
              width: "50%",
              height: "100%",
              position: "relative",
              overflow: "hidden",
            }}
          >
            <MarsTilemap />
            <GroundedSprite
              src="game-assets/Sprites/Mars/Relief/meseta-alta.png"
              x={240}
              y={220}
              size={225}
            />
            <GroundedSprite
              src="game-assets/Sprites/Mars/Rocks/roca-alta.png"
              x={720}
              y={260}
              size={150}
            />

            {/* Target Science Point #3 at (665, 505) */}
            <GroundedSprite
              src="game-assets/Sprites/Mars/SciencePoints/science_point_delta.png"
              x={665}
              y={505}
              size={150}
            >
              <div style={{ position: "absolute", left: -19, top: -19 }}>
                <PixelSprite
                  src={`game-assets/Sprites/Mars/Effects/Scan/scan_00${scanIdx}.png`}
                  size={188}
                />
              </div>
              <div style={{ position: "absolute", left: 48, top: -46 }}>
                <PixelSprite
                  src="game-assets/Sprites/Mars/Indicators/science_completed.png"
                  size={54}
                />
              </div>
            </GroundedSprite>

            {/* Rescued Rover Stops Cleanly at x=465 (200px left of Science Point at x=665 — Zero Overlap!) */}
            <GroundedSprite
              src={`game-assets/Sprites/Rover/DrivingV2/East/drive_east_00${driveIdx}.png`}
              x={interpolate(frame, [92, 165], [220, 465], {
                extrapolateRight: "clamp",
              })}
              y={525}
              size={185}
              shadowWidth={140}
              shadowHeight={38}
            >
              {/* Safe Mode / Telemetry Recovery Ring Around the Rover */}
              <div
                style={{
                  position: "absolute",
                  left: -16,
                  bottom: 4,
                  width: 216,
                  height: 62,
                  borderRadius: "50%",
                  border: "3px solid #4ade80",
                  boxShadow: "0 0 20px rgba(74, 222, 128, 0.75)",
                }}
              />
            </GroundedSprite>
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
          background:
            "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
          border: `4px solid ${!isRescue ? "#ef4444" : "#22c55e"}`,
          boxShadow: "0 14px 36px rgba(0,0,0,0.85)",
          textAlign: "right",
          zIndex: 90,
        }}
      >
        <div
          style={{
            fontFamily: FONTS.title,
            fontSize: 50,
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
          background:
            "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
          border: `3px solid ${!isRescue ? "#ef4444" : "#22c55e"}`,
          display: "flex",
          alignItems: "center",
          gap: 12,
          zIndex: 90,
        }}
      >
        <PixelSprite
          src={
            !isRescue
              ? "game-assets/Sprites/Icons/Resources/no_signal.png"
              : "game-assets/Sprites/Icons/Resources/safe_mode.png"
          }
          size={36}
        />
        <span
          style={{
            fontFamily: FONTS.title,
            fontSize: 21,
            color: !isRescue ? "#f87171" : "#4ade80",
          }}
        >
          {!isRescue
            ? "¡ALERTA: SIN SEÑAL Y PANELES A 0 E/s!"
            : "RELÉ ORBITAL + RESERVA DE MODO SEGURO (+0.8 E/s)"}
        </span>
      </div>

      {/* Bottom Dialogue Box */}
      <div
        style={{
          position: "absolute",
          bottom: 28,
          left: 120,
          right: 120,
          height: 150,
          background:
            "linear-gradient(180deg, rgba(11, 18, 36, 0.95) 0%, rgba(5, 9, 20, 0.98) 100%)",
          border: `3px solid ${!isRescue ? "#ef4444" : "#22c55e"}`,
          boxShadow: "0 18px 45px rgba(0,0,0,0.92)",
          display: "flex",
          alignItems: "center",
          padding: "16px 28px",
          gap: 22,
          zIndex: 90,
        }}
      >
        <div
          style={{
            width: 108,
            height: 108,
            background: "#090f20",
            border: `3px solid ${!isRescue ? "#ef4444" : "#22c55e"}`,
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
              fontSize: 22,
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
              fontSize: 21,
              fontWeight: 800,
              color: "#f8fafc",
              lineHeight: 1.3,
            }}
          >
            {!isRescue
              ? "«¡Comandante! El polvo bloqueó los paneles y saltan chispas de la antena. ¡Activando Modo Seguro!»"
              : "«¡Enlace restablecido desde el Centro de Control! El rover recupera energía con su reserva y completa la misión 3/3.»"}
          </div>
        </div>
      </div>

      <TacticalHudFrame alertMode={!isRescue} />
    </AbsoluteFill>
  );
};
