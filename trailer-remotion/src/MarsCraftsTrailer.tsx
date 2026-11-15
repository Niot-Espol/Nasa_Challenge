import React from 'react';
import { AbsoluteFill, Audio, Sequence, staticFile } from 'remotion';
import { Scene01_UniverseAwaits } from './scenes/Scene01_UniverseAwaits';
import { Scene02_MissionBegins } from './scenes/Scene02_MissionBegins';
import { Scene03_DesignYourMission } from './scenes/Scene03_DesignYourMission';
import { Scene04_TheLaunch } from './scenes/Scene04_TheLaunch';
import { Scene05_ExploreMars } from './scenes/Scene05_ExploreMars';
import { Scene06_MoonSecret } from './scenes/Scene06_MoonSecret';
import { Scene07_ControlRescue } from './scenes/Scene07_ControlRescue';
import { Scene08_GrandFinale } from './scenes/Scene08_GrandFinale';
import { ensureTrailerFonts } from './styles/theme';

export const MarsCraftsTrailer: React.FC = () => {
  ensureTrailerFonts();

  return (
    <AbsoluteFill style={{ backgroundColor: '#05070D' }}>
      {/* Banda sonora original de 45.0s sincronizada con las 8 escenas */}
      <Audio src={staticFile('audio/mars_crafts_trailer_ost.wav')} volume={0.85} />

      {/* Efectos de sonido reales del juego sincronizados al fotograma */}
      {/* Escena 2: pasos y diálogo en el Hub */}
      <Sequence from={198} durationInFrames={30} name="SFX_Hub_Dialogue">
        <Audio src={staticFile('audio/dialogue_blip.mp3')} volume={0.55} />
      </Sequence>

      {/* Escena 3: clics de ingeniería en el Taller */}
      <Sequence from={278} durationInFrames={20} name="SFX_Workshop_Click1">
        <Audio src={staticFile('audio/click.wav')} volume={0.6} />
      </Sequence>
      <Sequence from={330} durationInFrames={30} name="SFX_Workshop_Error">
        <Audio src={staticFile('audio/Generated/error_buzz.wav')} volume={0.55} />
      </Sequence>
      <Sequence from={390} durationInFrames={25} name="SFX_Workshop_Ready">
        <Audio src={staticFile('audio/star_ding.wav')} volume={0.65} />
      </Sequence>

      {/* Escena 4: Cuenta regresiva y rugido de despegue */}
      <Sequence from={450} durationInFrames={12} name="SFX_Countdown_3">
        <Audio src={staticFile('audio/countdown_beep.wav')} volume={0.65} />
      </Sequence>
      <Sequence from={462} durationInFrames={12} name="SFX_Countdown_2">
        <Audio src={staticFile('audio/countdown_beep.wav')} volume={0.65} />
      </Sequence>
      <Sequence from={474} durationInFrames={12} name="SFX_Countdown_1">
        <Audio src={staticFile('audio/countdown_beep.wav')} volume={0.75} />
      </Sequence>
      <Sequence from={486} durationInFrames={130} name="SFX_Rocket_Roar">
        <Audio src={staticFile('audio/rocket_roar.wav')} volume={0.8} />
      </Sequence>

      {/* Escena 5: Escaneo en Marte y tormenta */}
      <Sequence from={728} durationInFrames={45} name="SFX_Mars_Scan">
        <Audio src={staticFile('audio/scan.wav')} volume={0.65} />
      </Sequence>
      <Sequence from={775} durationInFrames={35} name="SFX_Mars_Complete">
        <Audio src={staticFile('audio/site_complete.wav')} volume={0.7} />
      </Sequence>

      {/* Escena 6: Descubrimiento de hielo en la Luna */}
      <Sequence from={928} durationInFrames={45} name="SFX_Moon_Ice_Scan">
        <Audio src={staticFile('audio/scan.wav')} volume={0.65} />
      </Sequence>
      <Sequence from={968} durationInFrames={22} name="SFX_Moon_Complete">
        <Audio src={staticFile('audio/site_complete.wav')} volume={0.75} />
      </Sequence>

      {/* Escena 7: Alerta de Modo Seguro -> Reconexión y 3 estrellas */}
      <Sequence from={995} durationInFrames={45} name="SFX_Safe_Mode_Alert">
        <Audio src={staticFile('audio/Generated/safe_mode.wav')} volume={0.7} />
      </Sequence>
      <Sequence from={1060} durationInFrames={35} name="SFX_Comms_Restored">
        <Audio src={staticFile('audio/Generated/comms_back.wav')} volume={0.75} />
      </Sequence>
      <Sequence from={1135} durationInFrames={45} name="SFX_Rescue_Success">
        <Audio src={staticFile('audio/star_ding.wav')} volume={0.8} />
      </Sequence>

      {/* =================================================================== */}
      {/* SECUENCIAS VISUALES (1350 FOTOGRAMAS EXACTOS = 45.00 SEGUNDOS)      */}
      {/* =================================================================== */}
      <Sequence
        from={0}
        durationInFrames={120}
        name="01_El_Universo_Nos_Espera (00-04s)"
      >
        <Scene01_UniverseAwaits />
      </Sequence>

      <Sequence
        from={120}
        durationInFrames={150}
        name="02_El_Comienzo_De_La_Mision (04-09s)"
      >
        <Scene02_MissionBegins />
      </Sequence>

      <Sequence
        from={270}
        durationInFrames={180}
        name="03_Disena_Tu_Mision (09-15s)"
      >
        <Scene03_DesignYourMission />
      </Sequence>

      <Sequence
        from={450}
        durationInFrames={210}
        name="04_El_Lanzamiento (15-22s)"
      >
        <Scene04_TheLaunch />
      </Sequence>

      <Sequence
        from={660}
        durationInFrames={180}
        name="05_Exploracion_De_Marte (22-28s)"
      >
        <Scene05_ExploreMars />
      </Sequence>

      <Sequence
        from={840}
        durationInFrames={150}
        name="06_El_Secreto_De_La_Luna (28-33s)"
      >
        <Scene06_MoonSecret />
      </Sequence>

      <Sequence
        from={990}
        durationInFrames={210}
        name="07_Rescate_Y_Resolucion (33-40s)"
      >
        <Scene07_ControlRescue />
      </Sequence>

      <Sequence
        from={1200}
        durationInFrames={150}
        name="08_Cierre_Cinematografico (40-45s)"
      >
        <Scene08_GrandFinale />
      </Sequence>
    </AbsoluteFill>
  );
};
