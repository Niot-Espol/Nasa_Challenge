import os
import wave
import numpy as np

SR = 44100
DURATION = 45.0
N_SAMPLES = int(SR * DURATION)

# MIDI note to frequency
def midi_to_freq(m: float) -> float:
    return 440.0 * (2.0 ** ((m - 69.0) / 12.0))

# Simple 1-pole low-pass filter (vectorized via scipy-free IIR loop in chunks or fast numpy lfilter equivalent)
def lowpass_1pole(sig: np.ndarray, cutoff_hz: float) -> np.ndarray:
    rc = 1.0 / (2.0 * np.pi * max(40.0, cutoff_hz))
    dt = 1.0 / SR
    alpha = dt / (rc + dt)
    out = np.zeros_like(sig)
    y = 0.0
    # Fast numba-free loop or block filter: since 45s is 1.98M samples, let's use a 5-tap FIR/moving-average cascade for instant vectorization!
    win = max(1, int(SR / (cutoff_hz * 2.2)))
    if win <= 1:
        return sig
    kernel = np.hanning(win * 2 + 1)
    kernel /= np.sum(kernel)
    return np.convolve(sig, kernel, mode="same")

def adsr_envelope(length: int, atk: float, dec: float, sus: float, rel: float) -> np.ndarray:
    t = np.linspace(0, length / SR, length, endpoint=False)
    dur = length / SR
    env = np.ones(length, dtype=np.float32) * sus
    atk_s = max(1, int(atk * SR))
    dec_s = max(1, int(dec * SR))
    rel_s = max(1, int(rel * SR))

    if atk_s < length:
        env[:atk_s] = np.linspace(0.0, 1.0, atk_s)
    else:
        return np.linspace(0.0, 1.0, length)

    end_dec = min(length, atk_s + dec_s)
    if end_dec > atk_s:
        env[atk_s:end_dec] = np.linspace(1.0, sus, end_dec - atk_s)

    if rel_s < length:
        env[-rel_s:] *= np.linspace(1.0, 0.0, rel_s)
    return env

def add_stereo(mix_L: np.ndarray, mix_R: np.ndarray, start_s: float, sig_L: np.ndarray, sig_R: np.ndarray):
    i0 = int(start_s * SR)
    if i0 >= N_SAMPLES or i0 + len(sig_L) <= 0:
        return
    s0 = max(0, -i0)
    d0 = max(0, i0)
    count = min(len(sig_L) - s0, N_SAMPLES - d0)
    if count <= 0:
        return
    mix_L[d0 : d0 + count] += sig_L[s0 : s0 + count]
    mix_R[d0 : d0 + count] += sig_R[s0 : s0 + count]

# 1. Warm Stereo Polysynth Pad (FTL / Little-Known Galaxy style)
def synth_pad_chord(midi_notes: list[int], start_s: float, dur_s: float, amp: float = 0.12, brightness: float = 1400.0):
    length = int(dur_s * SR)
    t = np.linspace(0, dur_s, length, endpoint=False)
    sig_L = np.zeros(length, dtype=np.float32)
    sig_R = np.zeros(length, dtype=np.float32)

    for idx, m in enumerate(midi_notes):
        f0 = midi_to_freq(m)
        # Detuned stereo oscillators + warm triangle fundamental
        f_L = f0 * (2.0 ** (-6.5 / 1200.0))
        f_R = f0 * (2.0 ** (6.5 / 1200.0))
        ph_L = 2.0 * np.pi * f_L * t + (idx * 1.1)
        ph_R = 2.0 * np.pi * f_R * t + (idx * 1.7)
        ph_C = 2.0 * np.pi * f0 * t

        # Soft rich saw (first 5 harmonics with 1/k^1.4 rolloff) + fundamental sine
        osc_L = 0.55 * np.sin(ph_C)
        osc_R = 0.55 * np.sin(ph_C)
        for k in range(1, 6):
            w = ((-1) ** (k + 1)) / (k ** 1.35)
            osc_L += 0.45 * w * np.sin(k * ph_L)
            osc_R += 0.45 * w * np.sin(k * ph_R)

        sig_L += osc_L / len(midi_notes)
        sig_R += osc_R / len(midi_notes)

    env = adsr_envelope(length, min(0.45, dur_s * 0.25), 0.3, 0.85, min(0.65, dur_s * 0.3))
    sig_L = lowpass_1pole(sig_L * env * amp, brightness)
    sig_R = lowpass_1pole(sig_R * env * amp, brightness)
    return start_s, sig_L, sig_R

# 2. 16-Bit Crystal FM Bell / Rhodes Pluck (with stereo pan)
def synth_fm_bell(midi_note: int, start_s: float, dur_s: float, amp: float = 0.11, pan: float = 0.0, glassy: bool = False):
    length = int(dur_s * SR)
    t = np.linspace(0, dur_s, length, endpoint=False)
    f0 = midi_to_freq(midi_note)
    ratio = 3.0 if glassy else 2.0
    mod_idx = np.exp(-t * 7.5) * (2.2 if glassy else 1.5)
    mod = np.sin(2.0 * np.pi * (f0 * ratio) * t) * mod_idx
    carrier = np.sin(2.0 * np.pi * f0 * t + mod) + 0.25 * np.sin(2.0 * np.pi * (f0 * 0.5) * t)
    env = np.exp(-t * (4.2 / max(0.25, dur_s))) * adsr_envelope(length, 0.006, 0.08, 0.7, 0.08)
    sig = carrier * env * amp
    gain_L = np.sqrt(0.5 * (1.0 - pan))
    gain_R = np.sqrt(0.5 * (1.0 + pan))
    return start_s, sig * gain_L, sig * gain_R

# 3. Expressive 16-Bit SNES/GBA Lead Synth (Pulse + Triangle + Vibrato)
def synth_16bit_lead(midi_note: float, start_s: float, dur_s: float, amp: float = 0.15, duty: float = 0.35):
    length = int(dur_s * SR)
    t = np.linspace(0, dur_s, length, endpoint=False)
    f0 = midi_to_freq(midi_note)
    # Gentle delayed vibrato (5.5 Hz, kicks in after 0.1s)
    vib_depth = np.clip((t - 0.08) / 0.15, 0.0, 1.0) * 0.0045
    inst_freq = f0 * (1.0 + vib_depth * np.sin(2.0 * np.pi * 5.5 * t))
    phase = 2.0 * np.pi * np.cumsum(inst_freq) / SR

    # Band-limited pulse wave (sum of 8 harmonics) + warm triangle body
    pulse = np.zeros(length, dtype=np.float32)
    for k in range(1, 9):
        ck = (2.0 / (k * np.pi)) * np.sin(k * np.pi * duty)
        pulse += ck * np.cos(k * phase)

    sub = 0.45 * np.sin(phase) + 0.2 * np.sin(0.5 * phase)
    raw = 0.65 * pulse + sub
    env = adsr_envelope(length, 0.015, 0.12, 0.78, min(0.12, dur_s * 0.35))
    filtered = lowpass_1pole(raw * env * amp, 3200.0)
    # Slight stereo widening via 11ms Haas/chorus offset
    delay_s = int(0.011 * SR)
    sig_L = filtered
    sig_R = np.pad(filtered, (delay_s, 0))[:length] * 0.92
    return start_s, sig_L, sig_R

# 4. Punchy 16-Bit Sub + Triangle Bass
def synth_bass_note(midi_note: int, start_s: float, dur_s: float, amp: float = 0.20):
    length = int(dur_s * SR)
    t = np.linspace(0, dur_s, length, endpoint=False)
    f0 = midi_to_freq(midi_note)
    ph = 2.0 * np.pi * f0 * t
    # Fundamental + 2nd & 3rd harmonic punch with fast filter envelope
    filt_env = np.exp(-t * 14.0)
    wave_sig = (
        0.70 * np.sin(ph)
        + 0.35 * np.sin(2.0 * ph) * (0.4 + 0.6 * filt_env)
        + 0.18 * np.sin(3.0 * ph) * filt_env
    )
    env = adsr_envelope(length, 0.008, 0.08, 0.72, 0.04)
    sig = wave_sig * env * amp
    return start_s, sig, sig

# 5. Authentic 16-Bit Drum Kit (Kick, Snare, Hi-Hat, Crash/Swell)
def synth_kick(start_s: float, amp: float = 0.32):
    dur_s = 0.22
    length = int(dur_s * SR)
    t = np.linspace(0, dur_s, length, endpoint=False)
    freq = 44.0 + 115.0 * np.exp(-t * 34.0)
    ph = 2.0 * np.pi * np.cumsum(freq) / SR
    click = np.exp(-t * 180.0) * 0.35 * np.sin(2.0 * np.pi * 900.0 * t)
    env = np.exp(-t * 12.5)
    sig = (np.sin(ph) * env + click) * amp
    return start_s, sig, sig

def synth_snare(start_s: float, amp: float = 0.20):
    dur_s = 0.20
    length = int(dur_s * SR)
    t = np.linspace(0, dur_s, length, endpoint=False)
    # Pitched 16-bit body (190 -> 130 Hz) + crisp filtered noise
    f_body = 130.0 + 70.0 * np.exp(-t * 40.0)
    ph = 2.0 * np.pi * np.cumsum(f_body) / SR
    body = np.sin(ph) * np.exp(-t * 22.0) * 0.55
    rng = np.random.default_rng(int(start_s * 1000) % 99991)
    noise = rng.standard_normal(length).astype(np.float32)
    noise = lowpass_1pole(noise, 4800.0) * np.exp(-t * 17.5) * 0.65
    sig = (body + noise) * amp
    return start_s, sig * 0.96, sig * 1.04

def synth_hat(start_s: float, open_hat: bool = False, amp: float = 0.07):
    dur_s = 0.22 if open_hat else 0.055
    length = int(dur_s * SR)
    t = np.linspace(0, dur_s, length, endpoint=False)
    rng = np.random.default_rng(int(start_s * 1000 + 7) % 99991)
    # High-frequency metallic shimmer (6 inharmonic square ratios + white noise)
    noise = rng.standard_normal(length).astype(np.float32)
    # High-pass via difference
    hp = np.diff(noise, prepend=0.0)
    decay = 11.0 if open_hat else 48.0
    sig = hp * np.exp(-t * decay) * amp
    return start_s, sig * 0.85, sig * 1.15

def synth_cymbal_swell(start_s: float, dur_s: float = 1.0, reverse: bool = True, amp: float = 0.11):
    length = int(dur_s * SR)
    t = np.linspace(0, dur_s, length, endpoint=False)
    rng = np.random.default_rng(int(start_s * 100 + 13) % 99991)
    noise = np.diff(rng.standard_normal(length).astype(np.float32), prepend=0.0)
    shimmer = noise * (0.6 + 0.4 * np.sin(2.0 * np.pi * 880.0 * t))
    env = (t / dur_s) ** 2.2 if reverse else np.exp(-t * (3.2 / dur_s))
    sig = lowpass_1pole(shimmer, 6500.0) * env * amp
    return start_s, sig * 0.9, sig * 1.1

def build_soundtrack():
    mix_L = np.zeros(N_SAMPLES, dtype=np.float32)
    mix_R = np.zeros(N_SAMPLES, dtype=np.float32)

    # Note helper (D Dorian / B minor / D Major heroic space palette)
    # MIDI: D2=38, E2=40, F#2=42, G2=43, A2=45, B2=47, C3=48, C#3=49, D3=50
    # D4=62, E4=64, F#4=66, G4=67, A4=69, B4=71, C5=72, C#5=73, D5=74, E5=76, F#5=78, G5=79, A5=81, B5=83, D6=86

    # =========================================================================
    # 1. CHORD PAD PROGRESSION ACROSS ALL 8 SCENES (0.0s -> 45.0s)
    # =========================================================================
    pad_schedule = [
        # Scene 1 (0-4s): El Universo Nos Espera (Cosmic Dmaj9 -> Gmaj7#11)
        ([50, 57, 62, 66, 69, 73], 0.0, 2.5, 0.14, 1300.0),
        ([43, 50, 59, 62, 66, 71], 2.5, 1.6, 0.16, 1700.0),
        # Scene 2 (4-9s): 01_Hub Space Center (Bm9 -> Gmaj7 -> Em9 -> Asus4)
        ([47, 54, 59, 62, 66, 69], 4.0, 2.5, 0.14, 1500.0),
        ([43, 50, 59, 62, 66, 69], 6.5, 1.3, 0.14, 1600.0),
        ([45, 52, 57, 62, 64, 69], 7.8, 1.3, 0.15, 1800.0),
        # Scene 3 (9-15s): 02_Workshop Engineering Hangar (Bm7 -> Gmaj7 -> Dmaj7 -> Asus4 -> A7)
        ([47, 54, 59, 62, 66], 9.0, 1.5, 0.15, 1700.0),
        ([43, 50, 59, 62, 67], 10.5, 1.5, 0.15, 1850.0),
        ([50, 57, 62, 66, 69], 12.0, 1.5, 0.16, 2000.0),
        ([45, 52, 57, 62, 64, 69], 13.5, 1.6, 0.18, 2400.0),
        # Scene 4 (15-22s): 03_Launch Ignition, Liftoff & Orbital Cruise
        ([50, 57, 62, 66, 69, 74], 15.0, 2.0, 0.19, 2600.0),
        ([43, 50, 59, 62, 67, 74], 17.0, 2.2, 0.19, 2500.0),
        ([47, 54, 59, 62, 66, 73], 19.2, 1.4, 0.16, 1800.0),
        ([45, 52, 57, 62, 64, 69], 20.6, 1.5, 0.16, 1900.0),
        # Scene 5 (22-28s): 04_Mars Jezero Crater & Delta Exploration (D Dorian / Heroic)
        ([50, 57, 62, 65, 69], 22.0, 2.0, 0.16, 1900.0),
        ([48, 55, 60, 64, 67, 72], 24.0, 2.0, 0.16, 2000.0),
        ([43, 50, 58, 62, 67], 26.0, 2.0, 0.16, 2000.0),
        # Scene 6 (28-33s): 04_Moon Cabeus Shadow (-240 C) — Crystalline & Mystical
        ([47, 54, 59, 62, 66, 73], 28.0, 2.5, 0.15, 1400.0),
        ([42, 49, 57, 61, 66, 73], 30.5, 2.5, 0.15, 1500.0),
        # Scene 7 (33-40s): Crisis in Sandstorm (33-36s) -> Triumphant Rescue (36-40s)
        ([47, 54, 59, 62, 65], 33.0, 1.5, 0.17, 2100.0),
        ([44, 52, 59, 62, 68], 34.5, 1.5, 0.18, 2300.0),
        ([43, 50, 59, 62, 66, 71, 74], 36.0, 2.0, 0.19, 2500.0),
        ([45, 52, 57, 61, 64, 69, 73], 38.0, 2.0, 0.20, 2700.0),
        # Scene 8 (40-45s): Grand Finale Mars Crafts (Triumphant Dmaj9 -> Gmaj7 -> Dmaj9 Resolve)
        ([50, 57, 62, 66, 69, 73, 74], 40.0, 2.0, 0.20, 2800.0),
        ([43, 50, 59, 62, 67, 71, 74], 42.0, 1.2, 0.19, 2600.0),
        ([50, 57, 62, 66, 69, 73, 74], 43.2, 1.8, 0.21, 2400.0),
    ]
    for notes, st, dur, amp, br in pad_schedule:
        s0, l, r = synth_pad_chord(notes, st, dur, amp, br)
        add_stereo(mix_L, mix_R, s0, l, r)

    # =========================================================================
    # 2. SPARKLING 16-BIT FM CRYSTAL ARPEGGIATOR (Throughout all 45s)
    # =========================================================================
    arp_patterns = [
        # (start_s, end_s, step_s, notes_cycle, glassy, amp)
        (0.0, 4.0, 0.125, [62, 66, 69, 73, 74, 73, 69, 66], True, 0.095),
        (4.0, 9.0, 0.125, [59, 62, 66, 69, 71, 69, 66, 62], False, 0.085),
        (9.0, 15.0, 0.125, [62, 66, 69, 74, 76, 74, 69, 66], False, 0.095),
        (15.0, 19.2, 0.125, [62, 69, 74, 78, 81, 78, 74, 69], False, 0.09),
        (19.2, 22.0, 0.125, [66, 69, 73, 74, 78, 74, 73, 69], True, 0.105),
        (22.0, 28.0, 0.125, [62, 65, 69, 72, 74, 72, 69, 65], False, 0.09),
        (28.0, 33.0, 0.125, [66, 71, 73, 78, 83, 78, 73, 71], True, 0.115),
        (33.0, 36.0, 0.100, [59, 62, 65, 68, 71, 68, 65, 62], False, 0.105),
        (36.0, 40.0, 0.125, [62, 66, 71, 74, 78, 74, 71, 66], True, 0.10),
        (40.0, 44.2, 0.125, [62, 66, 69, 73, 74, 78, 81, 86], True, 0.11),
    ]
    for st, end_s, step_s, cycle, glassy, amp in arp_patterns:
        t_cur = st
        idx = 0
        while t_cur < end_s - 0.02:
            note = cycle[idx % len(cycle)]
            pan = 0.45 * np.sin(idx * 1.3)
            s0, l, r = synth_fm_bell(note, t_cur, step_s * 1.6, amp=amp, pan=pan, glassy=glassy)
            add_stereo(mix_L, mix_R, s0, l, r)
            # Ping-pong stereo echo 0.25s later
            add_stereo(mix_L, mix_R, s0 + 0.25, r * 0.38, l * 0.38)
            t_cur += step_s
            idx += 1

    # =========================================================================
    # 3. HEROIC 16-BIT LEAD MELODY (SNES / GBA / FTL Style)
    # =========================================================================
    lead_notes = [
        # Scene 1: Cosmic Motif (0.4s - 3.8s)
        (69, 0.4, 0.8, 0.12, 0.4),
        (74, 1.2, 0.8, 0.13, 0.4),
        (73, 2.0, 0.5, 0.13, 0.4),
        (74, 2.5, 0.4, 0.15, 0.35),
        (78, 2.9, 0.4, 0.15, 0.35),
        (81, 3.3, 0.6, 0.16, 0.35),

        # Scene 2: 01_Hub Space Center Briefing (4.2s - 8.8s)
        (66, 4.2, 0.5, 0.13, 0.45),
        (69, 4.7, 0.5, 0.13, 0.45),
        (71, 5.2, 0.75, 0.14, 0.45),
        (74, 6.0, 0.5, 0.14, 0.45),
        (73, 6.5, 0.5, 0.14, 0.45),
        (71, 7.0, 0.5, 0.13, 0.45),
        (69, 7.5, 0.5, 0.14, 0.45),
        (76, 8.0, 0.85, 0.15, 0.45),

        # Scene 3: 02_Workshop Component Upgrades & Rollout (9.2s - 14.9s)
        (69, 9.2, 0.35, 0.14, 0.3),
        (71, 9.6, 0.35, 0.14, 0.3),
        (74, 10.0, 0.45, 0.15, 0.3),
        (71, 10.5, 0.35, 0.14, 0.3),
        (74, 10.9, 0.35, 0.15, 0.3),
        (76, 11.3, 0.65, 0.15, 0.3),
        (74, 12.0, 0.35, 0.15, 0.3),
        (78, 12.4, 0.35, 0.15, 0.3),
        (81, 12.8, 0.65, 0.16, 0.3),
        # Rollout ascending fanfare (13.5s - 14.9s)
        (74, 13.5, 0.3, 0.16, 0.28),
        (76, 13.8, 0.3, 0.16, 0.28),
        (78, 14.1, 0.3, 0.17, 0.28),
        (81, 14.4, 0.55, 0.18, 0.28),

        # Scene 4: 03_Launch Ignition & Liftoff Anthem (15.0s - 21.8s)
        (74, 15.0, 0.6, 0.18, 0.25),
        (81, 15.6, 0.6, 0.19, 0.25),
        (78, 16.2, 0.6, 0.18, 0.25),
        (74, 16.8, 0.45, 0.18, 0.25),
        (79, 17.25, 0.45, 0.19, 0.25),
        (78, 17.7, 0.5, 0.18, 0.25),
        (76, 18.2, 0.5, 0.18, 0.25),
        (74, 18.7, 0.5, 0.17, 0.25),
        # Shot 4C: Terra Orbital Telemetry & Cruise Theme
        (73, 19.2, 0.65, 0.15, 0.4),
        (74, 19.85, 0.65, 0.15, 0.4),
        (78, 20.5, 0.65, 0.16, 0.4),
        (76, 21.15, 0.75, 0.16, 0.4),

        # Scene 5: 04_Mars Jezero Exploration Theme (22.0s - 27.8s)
        (62, 22.0, 0.45, 0.16, 0.32),
        (69, 22.45, 0.45, 0.16, 0.32),
        (72, 22.9, 0.55, 0.17, 0.32),
        (69, 23.45, 0.5, 0.16, 0.32),
        (74, 24.0, 0.5, 0.17, 0.32),
        (72, 24.5, 0.5, 0.16, 0.32),
        (69, 25.0, 0.5, 0.16, 0.32),
        (67, 25.5, 0.5, 0.16, 0.32),
        (69, 26.0, 0.6, 0.17, 0.32),
        (74, 26.6, 0.6, 0.18, 0.32),
        (76, 27.2, 0.7, 0.18, 0.32),

        # Scene 6: 04_Moon Cabeus Shadow Ice Discovery (28.0s - 32.8s)
        (78, 28.2, 0.75, 0.14, 0.48),
        (74, 28.95, 0.75, 0.14, 0.48),
        (73, 29.7, 0.8, 0.14, 0.48),
        (78, 30.5, 0.6, 0.15, 0.48),
        (81, 31.1, 0.6, 0.16, 0.48),
        (83, 31.7, 1.1, 0.16, 0.48),

        # Scene 7A: Crisis in Sandstorm (33.0s - 35.9s)
        (71, 33.0, 0.35, 0.16, 0.25),
        (74, 33.35, 0.35, 0.16, 0.25),
        (71, 33.7, 0.35, 0.16, 0.25),
        (68, 34.05, 0.45, 0.17, 0.25),
        (71, 34.5, 0.45, 0.17, 0.25),
        (74, 34.95, 0.45, 0.18, 0.25),
        (76, 35.4, 0.55, 0.19, 0.25),

        # Scene 7B: Safe Mode & Mission Control Rescue Climax (36.0s - 39.9s)
        (78, 36.0, 0.5, 0.18, 0.30),
        (79, 36.5, 0.5, 0.18, 0.30),
        (81, 37.0, 0.6, 0.19, 0.30),
        (78, 37.6, 0.4, 0.18, 0.30),
        (81, 38.0, 0.6, 0.19, 0.30),
        (83, 38.6, 0.6, 0.20, 0.30),
        (85, 39.2, 0.75, 0.20, 0.30),

        # Scene 8: Grand Finale Mars Crafts Theme (40.0s - 44.8s)
        (86, 40.0, 0.9, 0.20, 0.32),
        (81, 40.9, 0.5, 0.18, 0.32),
        (78, 41.4, 0.6, 0.18, 0.32),
        (79, 42.0, 0.6, 0.19, 0.32),
        (83, 42.6, 0.6, 0.19, 0.32),
        (86, 43.2, 1.65, 0.21, 0.32),
    ]
    for m_note, st, dur, amp, duty in lead_notes:
        s0, l, r = synth_16bit_lead(m_note, st, dur, amp=amp, duty=duty)
        add_stereo(mix_L, mix_R, s0, l, r)
        # Soft stereo delay tail on the lead
        add_stereo(mix_L, mix_R, s0 + 0.18, r * 0.25, l * 0.25)

    # =========================================================================
    # 4. DRIVING 16-BIT BASSLINE (Enters at 4.0s)
    # =========================================================================
    bass_sections = [
        # (start_s, end_s, step_s, root_midi)
        (4.0, 6.5, 0.5, 35),   # B1
        (6.5, 7.8, 0.5, 43),   # G2
        (7.8, 9.0, 0.5, 45),   # A2
        # Scene 3: 8th-note driving engineering bass
        (9.0, 10.5, 0.25, 35),
        (10.5, 12.0, 0.25, 43),
        (12.0, 13.5, 0.25, 38),
        (13.5, 15.0, 0.25, 45),
        # Scene 4: Liftoff & Cruise
        (15.0, 17.0, 0.25, 38),
        (17.0, 19.2, 0.25, 43),
        (19.2, 20.6, 0.5, 35),
        (20.6, 22.0, 0.5, 45),
        # Scene 5: Mars Exploration
        (22.0, 24.0, 0.25, 38),
        (24.0, 26.0, 0.25, 36),
        (26.0, 28.0, 0.25, 43),
        # Scene 6: Moon Shadow (deep half-note sub pulses)
        (28.0, 30.5, 0.5, 35),
        (30.5, 33.0, 0.5, 42),
        # Scene 7: Crisis & Rescue
        (33.0, 34.5, 0.25, 35),
        (34.5, 36.0, 0.25, 40),
        (36.0, 38.0, 0.25, 43),
        (38.0, 40.0, 0.25, 45),
        # Scene 8: Grand Finale
        (40.0, 42.0, 0.25, 38),
        (42.0, 43.2, 0.25, 43),
        (43.2, 44.8, 1.6, 38),
    ]
    for st, end_s, step_s, root in bass_sections:
        t_cur = st
        step_i = 0
        while t_cur < end_s - 0.02:
            # Octave jump on off-beats for 16-bit synthwave bounce
            note = root + (12 if (step_s <= 0.25 and step_i % 2 == 1) else 0)
            s0, l, r = synth_bass_note(note, t_cur, step_s * 0.9, amp=0.19)
            add_stereo(mix_L, mix_R, s0, l, r)
            t_cur += step_s
            step_i += 1

    # =========================================================================
    # 5. 16-BIT PERCUSSION & SCENE TRANSITION CYMBAL SWELLS
    # =========================================================================
    # Scene transition swells & crash accents
    for swell_t in [1.5, 3.0, 8.0, 14.0, 18.2, 21.0, 27.0, 32.0, 35.0, 39.0]:
        s0, l, r = synth_cymbal_swell(swell_t, dur_s=0.95, reverse=True, amp=0.09)
        add_stereo(mix_L, mix_R, s0, l, r)
    for crash_t in [2.5, 4.0, 9.0, 15.0, 19.2, 22.0, 28.0, 33.0, 36.0, 40.0, 43.2]:
        s0, l, r = synth_cymbal_swell(crash_t, dur_s=1.1, reverse=False, amp=0.10)
        add_stereo(mix_L, mix_R, s0, l, r)

    # Beat grid at 120 BPM (1 beat = 0.5s) from 4.0s to 43.2s
    beat_t = 4.0
    beat_idx = 0
    while beat_t < 43.2:
        in_moon = 28.0 <= beat_t < 33.0
        in_hub = 4.0 <= beat_t < 9.0

        # Kick on every beat (or beats 1 & 3 in Hub/Moon)
        if not in_moon or (beat_idx % 2 == 0):
            s0, l, r = synth_kick(beat_t, amp=0.24 if in_hub else 0.30)
            add_stereo(mix_L, mix_R, s0, l, r)

        # Snare on backbeats (odd beats: 2 and 4)
        if beat_idx % 2 == 1 and not in_moon:
            s0, l, r = synth_snare(beat_t, amp=0.14 if in_hub else 0.21)
            add_stereo(mix_L, mix_R, s0, l, r)

        # 8th-note & 16th-note Hi-Hats
        s0, l, r = synth_hat(beat_t, open_hat=False, amp=0.055)
        add_stereo(mix_L, mix_R, s0, l, r)
        s0, l, r = synth_hat(beat_t + 0.25, open_hat=(beat_idx % 2 == 1), amp=0.065)
        add_stereo(mix_L, mix_R, s0, l, r)

        if not in_hub and not in_moon:
            s0, l, r = synth_hat(beat_t + 0.125, open_hat=False, amp=0.038)
            add_stereo(mix_L, mix_R, s0, l, r)
            s0, l, r = synth_hat(beat_t + 0.375, open_hat=False, amp=0.042)
            add_stereo(mix_L, mix_R, s0, l, r)

        beat_t += 0.5
        beat_idx += 1

    # Exciting snare fill before Liftoff (14.25s - 15.0s) and before Final Climax (39.25s - 40.0s)
    for fill_start in [14.25, 35.25, 39.25]:
        for k in range(6):
            s0, l, r = synth_snare(fill_start + k * 0.125, amp=0.12 + k * 0.02)
            add_stereo(mix_L, mix_R, s0, l, r)

    # Master soft-clipping limiter & fade-out over final 0.5s
    fade_len = int(0.55 * SR)
    mix_L[-fade_len:] *= np.linspace(1.0, 0.0, fade_len)
    mix_R[-fade_len:] *= np.linspace(1.0, 0.0, fade_len)

    stereo = np.stack([mix_L, mix_R], axis=-1)
    peak = np.max(np.abs(stereo))
    if peak > 0:
        stereo = np.tanh(stereo / peak * 1.35) / np.tanh(1.35) * 0.92

    pcm16 = (stereo * 32767.0).astype(np.int16)
    return pcm16

if __name__ == "__main__":
    out_wav = r"c:\Users\José Checa\Documents\6SEMESTRE\NASA\Mars_Crafts_Trailer\public\audio\mars_crafts_trailer_ost.wav"
    os.makedirs(os.path.dirname(out_wav), exist_ok=True)
    pcm = build_soundtrack()
    with wave.open(out_wav, "wb") as wf:
        wf.setnchannels(2)
        wf.setsampwidth(2)
        wf.setframerate(SR)
        wf.writeframes(pcm.tobytes())
    print("Generated stereo 16-bit soundtrack:", out_wav, os.path.getsize(out_wav), "bytes")
