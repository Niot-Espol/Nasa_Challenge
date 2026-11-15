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

export const Scene02_MissionBegins: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  // Camera pans across the EXPANDED REAL GAME HUB SCENARIO (Generator Sector -> Central Command -> Workshop Airlock)
  const isPart2 = frame >= 75;
  const camScale = interpolate(frame, [0, 75, 150], [1.05, 1.14, 1.22], {
    extrapolateRight: "clamp",
  });
  const camX = interpolate(frame, [0, 75, 150], [120, 0, -90], {
    extrapolateRight: "clamp",
  });

  const vegaIdx = Math.floor(frame / 4) % 9;
  const vegaFrame = `game-assets/Sprites/Characters/DrElenaVega/Idle/South/vega-idle-south_00${vegaIdx}.png`;

  const bimoIdx = Math.floor(frame / 3) % 4;
  const bimoFrame = `game-assets/Sprites/Characters/BIMO/Move/East/move_east_00${bimoIdx}.png`;

  const mateoIdx = Math.floor(frame / 6) % 2;
  const lunaIdx = Math.floor(frame / 6) % 2;

  const runIdx = Math.floor(frame / 3) % 8;
  const playerFrame = `game-assets/Sprites/Characters/AstronautStocky/Running/East/frame_00${runIdx}.png`;

  const playerX = interpolate(frame, [0, 150], [380, 1080], {
    extrapolateRight: "clamp",
  });
  const bimoX = interpolate(frame, [0, 150], [260, 950], {
    extrapolateRight: "clamp",
  });

  const dialogPop = spring({
    frame: frame - 8,
    fps,
    config: { damping: 14, stiffness: 150 },
  });

  return (
    <AbsoluteFill style={{ backgroundColor: "#0a0f1d", overflow: "hidden" }}>
      <DiagonalWipeIn color="#38bdf8" />
      <ImpactFlash triggerFrame={75} duration={7} color="#38bdf8" maxOpacity={0.55} />

      {/* REAL GAME SCENARIO: 01_Hub Space Center built with LabTilesPNG + Hub Structure & Interior Props */}
      <AbsoluteFill
        style={{
          transform: `translate(${camX}px, 0px) scale(${camScale})`,
          transformOrigin: "center center",
        }}
      >
        {/* Main Metallic Tilemap Floor (floor_panels.png) */}
        <AbsoluteFill
          style={{
            backgroundImage: `url(${staticFile(
              "game-assets/Tilesets/Hub/LabTilesPNG/floor_panels.png"
            )})`,
            backgroundSize: "96px 96px",
            imageRendering: "pixelated",
          }}
        />

        {/* Central Engineering Walkway (floor_grate.png) */}
        <div
          style={{
            position: "absolute",
            left: 0,
            right: 0,
            top: 460,
            height: 220,
            backgroundImage: `url(${staticFile(
              "game-assets/Tilesets/Hub/LabTilesPNG/floor_grate.png"
            )})`,
            backgroundSize: "96px 96px",
            imageRendering: "pixelated",
            borderTop: "4px solid #38bdf8",
            borderBottom: "4px solid #38bdf8",
          }}
        />

        {/* Top Structural Wall built with real Hub wall panels & airlocks */}
        <div
          style={{
            position: "absolute",
            top: 0,
            left: 0,
            right: 0,
            height: 230,
            background: "linear-gradient(180deg, #0f172a 0%, #1e293b 100%)",
            borderBottom: "8px solid #334155",
            display: "flex",
            alignItems: "flex-end",
            justifyContent: "space-around",
            padding: "0 40px",
          }}
        >
          <PixelSprite src="game-assets/Sprites/Hub/Structure/muro-panel-a.png" size={180} />
          <PixelSprite src="game-assets/Sprites/Hub/Structure/esclusa-cian.png" size={210} />
          <PixelSprite src="game-assets/Sprites/Hub/Structure/muro-panel-b.png" size={180} />
          <PixelSprite src="game-assets/Sprites/Hub/Structure/muro-panel-c.png" size={180} />
          <PixelSprite src="game-assets/Sprites/Hub/Structure/esclusa-naranja.png" size={210} />
        </div>

        {/* SECTOR 1 (LEFT): Power Generator & Reactor Bay (Misión 1 Generator Repair!) */}
        <div style={{ position: "absolute", left: 130, top: 210 }}>
          <PixelSprite src="game-assets/Sprites/Hub/Interior/reactor-cian.png" size={220} />
          {/* Reactor Energy Pulse */}
          <div
            style={{
              position: "absolute",
              left: 20,
              top: 20,
              width: 180,
              height: 180,
              borderRadius: "50%",
              background:
                "radial-gradient(circle, rgba(56,189,248,0.6) 0%, transparent 70%)",
              opacity: 0.6 + 0.4 * Math.sin(frame * 0.35),
            }}
          />
        </div>
        <div style={{ position: "absolute", left: 350, top: 260 }}>
          <PixelSprite src="game-assets/Sprites/Hub/Interior/servidor.png" size={150} />
        </div>
        <div style={{ position: "absolute", left: 140, top: 690 }}>
          <PixelSprite src="game-assets/Sprites/Hub/Props/tanques-gemelos.png" size={155} />
        </div>

        {/* SECTOR 2 (CENTER): Mission Control Consoles & Research Station */}
        <div style={{ position: "absolute", left: 680, top: 240 }}>
          <PixelSprite src="game-assets/Sprites/Hub/Interior/consola-control.png" size={210} />
        </div>
        <div style={{ position: "absolute", left: 930, top: 240 }}>
          <PixelSprite src="game-assets/Sprites/Hub/Interior/consola-investigacion.png" size={210} />
        </div>
        <div style={{ position: "absolute", left: 790, top: 690 }}>
          <PixelSprite src="game-assets/Sprites/Hub/Interior/mesa-laboratorio.png" size={180} />
        </div>

        {/* SECTOR 3 (RIGHT): Diagnostic Station, Hydroponics & Workshop Gate */}
        <div style={{ position: "absolute", left: 1290, top: 235 }}>
          <PixelSprite src="game-assets/Sprites/Hub/Interior/estacion-diagnostico.png" size={210} />
        </div>
        <div style={{ position: "absolute", left: 1560, top: 250 }}>
          <PixelSprite src="game-assets/Sprites/Hub/Interior/cultivo-hidroponico.png" size={190} />
        </div>
        <div style={{ position: "absolute", left: 1420, top: 690 }}>
          <PixelSprite src="game-assets/Sprites/Hub/Interior/consola-logistica.png" size={185} />
        </div>

        {/* ALL 5 GAME CHARACTERS ALIVE IN THE HUB SCENARIO */}
        {/* Mateo Rojas by the Reactor & Server */}
        <div style={{ position: "absolute", left: 450, top: 380 }}>
          <PixelSprite
            src={`game-assets/Sprites/Characters/Mateo/Idle/South/idle_south_00${mateoIdx}.png`}
            size={145}
          />
        </div>

        {/* Dra. Elena Vega at Central Mission Control */}
        <div style={{ position: "absolute", left: 880, top: 380 }}>
          <PixelSprite src={vegaFrame} size={150} />
        </div>

        {/* Luna Torres at Operations & Comms */}
        <div style={{ position: "absolute", left: 1220, top: 390 }}>
          <PixelSprite
            src={`game-assets/Sprites/Characters/Luna/Idle/South/idle_south_00${lunaIdx}.png`}
            size={145}
          />
        </div>

        {/* Commander & BIMO walking along the grated corridor */}
        <div style={{ position: "absolute", left: playerX, top: 495 }}>
          <PixelSprite src={playerFrame} size={155} />
        </div>
        <div
          style={{
            position: "absolute",
            left: bimoX,
            top: 515 + Math.sin(frame * 0.35) * 8,
          }}
        >
          <PixelSprite src={bimoFrame} size={125} />
        </div>
      </AbsoluteFill>

      <ForegroundParticles
        color="#38bdf8"
        secondaryColor="#facc15"
        count={16}
        speedY={-1.8}
        speedX={1.0}
      />

      {/* Zone Arrival Banner (ZoneBanner style from 01_Hub) */}
      <div
        style={{
          position: "absolute",
          top: 32,
          left: 44,
          padding: "12px 26px",
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
          imageRendering: "pixelated",
          border: "3px solid #38bdf8",
          boxShadow: "0 12px 32px rgba(0,0,0,0.85)",
          zIndex: 65,
        }}
      >
        <div
          style={{
            fontFamily: FONTS.title,
            fontSize: 28,
            color: "#38bdf8",
            letterSpacing: 2,
          }}
        >
          01_HUB // CENTRO ESPACIAL MARS CRAFTS (REACTOR • CONTROL • TALLER)
        </div>
      </div>

      {/* Authentic Game Dialogue Box using UI/Dialogue/dialogue_frame.png */}
      <div
        style={{
          position: "absolute",
          bottom: 30,
          left: 130,
          right: 130,
          height: 158,
          transform: `scale(${dialogPop})`,
          background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
          imageRendering: "pixelated",
          border: "3px solid #38bdf8",
          boxShadow: "0 18px 45px rgba(0,0,0,0.92)",
          display: "flex",
          alignItems: "center",
          padding: "18px 30px",
          gap: 24,
          zIndex: 65,
        }}
      >
        <div
          style={{
            width: 114,
            height: 114,
            background: "linear-gradient(180deg, rgba(11, 18, 36, 0.94) 0%, rgba(5, 9, 20, 0.97) 100%)",
            imageRendering: "pixelated",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            flexShrink: 0,
          }}
        >
          <PixelSprite
            src={
              !isPart2
                ? "game-assets/Sprites/Portraits/DrElenaVega/vega-portrait-happy.png"
                : "game-assets/Sprites/Portraits/BIMO/bimo_happy.png"
            }
            size={98}
          />
        </div>

        <div style={{ flex: 1 }}>
          <div
            style={{
              fontFamily: FONTS.title,
              fontSize: 24,
              color: !isPart2 ? "#38bdf8" : "#facc15",
              letterSpacing: 1.5,
              marginBottom: 6,
            }}
          >
            {!isPart2
              ? "DRA. ELENA VEGA — BRIEFING DE LAS 3 EXPEDICIONES"
              : "BIMO & MATEO ROJAS — ACCESO AL TALLER DE INGENIERÍA DESBLOQUEADO"}
          </div>
          <div
            style={{
              fontFamily: FONTS.body,
              fontSize: 23,
              fontWeight: 800,
              color: "#f8fafc",
              lineHeight: 1.3,
            }}
          >
            {!isPart2
              ? "«Bienvenido al Centro Espacial. Estudiaremos la Tierra con el satélite Terra, el hielo polar de la Luna y el cráter Jezero en Marte.»"
              : "«¡Reactor en línea y esclusa abierta, Comandante! Vamos al Taller a ensamblar nuestro satélite y el rover.»"}
          </div>
        </div>
      </div>

      <TacticalHudFrame />
    </AbsoluteFill>
  );
};
