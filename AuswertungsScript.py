"""
Python-Skript zur Auswertung der Birdsong-Log-Daten.

Berechnet:
- n pro Messzeitpunkt
- deskriptive Statistik für Pre, Mid und Post
- Boxplot der Erfolgsraten
- gepaarte Pre-Post-Analyse mit
  - Shapiro-Wilk-Test
  - t-Test für verbundene Stichproben
  - Wilcoxon-Vorzeichen-Rang-Test
  - Cohen's dz
- vogel-spezifische Trefferquoten
- absolute und normalisierte Verwechslungsmatrizen
"""

import os
import glob
import re
from datetime import datetime
from collections import defaultdict

import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns
from scipy import stats

# =========================
# Konfiguration
# =========================
DATA_DIR = r"<Pfad_zum_Datenordner_in_dem_die_CSV_Datein_liegen>"
OUT_DIR = os.path.join(DATA_DIR, "analysis_output")

PRE_GAME_NAME = "BeginningQuiz"
MID_GAME_NAME = "BirdQuiz"
POST_GAME_NAME = "EndQuiz"

MID_LEVEL_BUTTON = 6
MID_LEVEL = 4

def ensure_dir(path: str) -> None:
    os.makedirs(path, exist_ok=True)

def parse_decimal_comma(value: str) -> float:
    return float(value.strip().replace(".", "").replace(",", "."))

def parse_timestamp(line: str):
    m = re.search(r"\[(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})\]", line)
    if not m:
        return None
    return datetime.strptime(m.group(1), "%Y-%m-%d %H:%M:%S")

def extract_quiz_metrics(
    lines,
    game_name: str,
    level_button: int = None,
    level: int = None
):
    candidates = []

    for line in lines:
        if f"END Game: {game_name}" not in line:
            continue

        if level_button is not None and not re.search(rf"LevelButtonNumber:\s*{level_button}\b", line):
            continue
        if level is not None and not re.search(rf"Level:\s*{level}\b", line):
            continue

        ts = parse_timestamp(line)
        candidates.append((ts, line))

    if not candidates:
        return None

    candidates.sort(key=lambda x: x[0] if x[0] else datetime.max)
    line = candidates[0][1]  # immer erster passender Eintrag

    sr = re.search(r"successrate:\s*([0-9]+(?:,[0-9]+)?)", line)
    ca = re.search(r"correctAnswers:\s*(\d+)", line)
    aq = re.search(r"amountOfQuestions:\s*(\d+)", line)

    if not (sr and ca and aq):
        return None

    return {
        "successrate": parse_decimal_comma(sr.group(1)),
        "correct": int(ca.group(1)),
        "questions": int(aq.group(1)),
    }

def extract_quiz_items(lines, game_name: str):
    items = []

    for line in lines:
        if f"Game: {game_name}" not in line:
            continue
        if not all(k in line for k in ["questionedBird:", "givenAnswer:", "wasAnswerCorrect:"]):
            continue

        q = re.search(r"questionedBird:\s*([^,]+)", line)
        a = re.search(r"givenAnswer:\s*([^,]+)", line)
        c = re.search(r"wasAnswerCorrect:\s*(True|False)", line)
        opts = re.findall(r"answerPossibility\d+:\s*([^,]+)", line)

        if not (q and a and c):
            continue

        items.append({
            "questionedBird": q.group(1).strip(),
            "givenAnswer": a.group(1).strip(),
            "correct": c.group(1).strip(),
            "options": [o.strip() for o in opts]
        })

    return items

def summary_stats(values):
    arr = np.array([v for v in values if v is not None], dtype=float)
    if arr.size == 0:
        return {"n": 0, "mean": np.nan, "sd": np.nan, "median": np.nan, "iqr": np.nan}

    q25, q75 = np.percentile(arr, [25, 75])
    return {
        "n": int(arr.size),
        "mean": float(np.mean(arr)),
        "sd": float(np.std(arr, ddof=1)) if arr.size > 1 else 0.0,
        "median": float(np.median(arr)),
        "iqr": float(q75 - q25),
    }

def cohen_dz(pre, post):
    diff = post - pre
    sd = np.std(diff, ddof=1)
    return 0.0 if sd == 0 else float(np.mean(diff) / sd)

def main():
    ensure_dir(OUT_DIR)

    files = sorted(glob.glob(os.path.join(DATA_DIR, "*.csv")))
    if not files:
        raise FileNotFoundError(f"Keine CSV-Dateien gefunden in: {DATA_DIR}")

    pre_sr, mid_sr, post_sr = [], [], []
    pre_items_all, post_items_all = [], []

    paired_pre_correct = paired_pre_questions = 0
    paired_post_correct = paired_post_questions = 0

    for fp in files:
        with open(fp, "r", encoding="utf-8", errors="replace") as f:
            lines = [line.strip() for line in f if line.strip()]

        pre = extract_quiz_metrics(lines, PRE_GAME_NAME)
        mid = extract_quiz_metrics(lines, MID_GAME_NAME, MID_LEVEL_BUTTON, MID_LEVEL)
        post = extract_quiz_metrics(lines, POST_GAME_NAME)

        pre_sr.append(pre["successrate"] if pre else None)
        mid_sr.append(mid["successrate"] if mid else None)
        post_sr.append(post["successrate"] if post else None)

        if pre:
            pre_items_all.extend(extract_quiz_items(lines, PRE_GAME_NAME))
        if post:
            post_items_all.extend(extract_quiz_items(lines, POST_GAME_NAME))

        if pre and post:
            paired_pre_correct += pre["correct"]
            paired_pre_questions += pre["questions"]
            paired_post_correct += post["correct"]
            paired_post_questions += post["questions"]

    pre_clean = [x for x in pre_sr if x is not None]
    mid_clean = [x for x in mid_sr if x is not None]
    post_clean = [x for x in post_sr if x is not None]

    print("=== Teilnahme ===")
    print(f"Pre : {len(pre_clean)}/{len(files)}")
    print(f"Mid : {len(mid_clean)}/{len(files)}")
    print(f"Post: {len(post_clean)}/{len(files)}\n")

    plt.figure()
    plt.boxplot([pre_clean, mid_clean, post_clean], labels=["Pre", "Mid", "Post"])
    plt.ylabel("Successrate (%)")
    plt.title("Birdsong Quest – Successrates")
    plt.tight_layout()
    plt.savefig(os.path.join(OUT_DIR, "boxplot_pre_mid_post.png"), dpi=200)
    plt.close()

    pre_stats = summary_stats(pre_clean)
    mid_stats = summary_stats(mid_clean)
    post_stats = summary_stats(post_clean)

    print("=== Deskriptive Statistik ===")
    print(f"Pre : n={pre_stats['n']}, mean={pre_stats['mean']:.2f}, sd={pre_stats['sd']:.2f}, median={pre_stats['median']:.2f}, iqr={pre_stats['iqr']:.2f}")
    print(f"Mid : n={mid_stats['n']}, mean={mid_stats['mean']:.2f}, sd={mid_stats['sd']:.2f}, median={mid_stats['median']:.2f}, iqr={mid_stats['iqr']:.2f}")
    print(f"Post: n={post_stats['n']}, mean={post_stats['mean']:.2f}, sd={post_stats['sd']:.2f}, median={post_stats['median']:.2f}, iqr={post_stats['iqr']:.2f}\n")

    print("=== Absolute Gesamtleistung (nur gepaarte Fälle) ===")
    if paired_pre_questions > 0:
        print(f"Pre : {paired_pre_correct} von {paired_pre_questions} korrekt ({paired_pre_correct / paired_pre_questions * 100:.2f}%)")
    if paired_post_questions > 0:
        print(f"Post: {paired_post_correct} von {paired_post_questions} korrekt ({paired_post_correct / paired_post_questions * 100:.2f}%)")
    print()

    paired_pre = np.array([a for a, b in zip(pre_sr, post_sr) if a is not None and b is not None], dtype=float)
    paired_post = np.array([b for a, b in zip(pre_sr, post_sr) if a is not None and b is not None], dtype=float)

    print("=== Gepaarte Pre-Post-Analyse ===")
    print(f"n = {paired_pre.size}")

    if paired_pre.size >= 2:
        diff = paired_post - paired_pre

        plt.figure()
        plt.hist(diff, bins="auto", edgecolor="black")
        plt.title("Histogramm: Differenzen (Post - Pre)")
        plt.xlabel("Differenz der Successrate")
        plt.ylabel("Häufigkeit")
        plt.tight_layout()
        plt.savefig(os.path.join(OUT_DIR, "histogram_diff_pre_post.png"), dpi=200)
        plt.close()

        print(f"Mean Pre  = {np.mean(paired_pre):.2f}%")
        print(f"Mean Post = {np.mean(paired_post):.2f}%")
        print(f"Mean Diff = {np.mean(diff):.2f} %-Punkte")
        print(f"SD Diff   = {np.std(diff, ddof=1):.2f}")

        shapiro_stat, shapiro_p = stats.shapiro(diff)
        t_stat, t_p = stats.ttest_rel(paired_post, paired_pre)
        wilcoxon_stat, wilcoxon_p = stats.wilcoxon(paired_post, paired_pre)
        dz = cohen_dz(paired_pre, paired_post)

        print(f"Shapiro-Wilk-Test: W = {shapiro_stat:.3f}, p = {shapiro_p:.6f}")
        print(f"t-Test (verbundene Stichproben): t = {t_stat:.3f}, p = {t_p:.6f}")
        print(f"Wilcoxon-Vorzeichen-Rang-Test: W = {wilcoxon_stat:.3f}, p = {wilcoxon_p:.6f}")
        print(f"Cohen's dz = {dz:.3f}")
    else:
        print("Nicht genug gepaarte Daten für inferenzstatistische Tests.")
    print()

    def bird_accuracy(items):
        result = defaultdict(lambda: [0, 0])
        for item in items:
            bird = item["questionedBird"]
            result[bird][1] += 1
            if item["correct"] == "True":
                result[bird][0] += 1
        return result

    pre_birds = bird_accuracy(pre_items_all)
    post_birds = bird_accuracy(post_items_all)

    bird_rows = []
    all_birds = sorted(set(pre_birds.keys()) | set(post_birds.keys()))

    for bird in all_birds:
        c1, n1 = pre_birds.get(bird, [0, 0])
        c2, n2 = post_birds.get(bird, [0, 0])
        p1 = c1 / n1 if n1 else np.nan
        p2 = c2 / n2 if n2 else np.nan
        d = p2 - p1 if not (np.isnan(p1) or np.isnan(p2)) else np.nan
        bird_rows.append((bird, p1, n1, p2, n2, d))

    print("=== Vogel-spezifische Trefferquoten ===")
    for bird, p1, n1, p2, n2, d in sorted(bird_rows, key=lambda x: np.nan_to_num(x[3], nan=1e9)):
        print(f"{bird:15s} post={p2*100:6.2f}% (n={n2}), pre={p1*100 if not np.isnan(p1) else np.nan:6.2f}% (n={n1}), delta={d*100:6.2f}")
    print()

    with open(os.path.join(OUT_DIR, "bird_difficulty_pre_post.txt"), "w", encoding="utf-8") as f:
        f.write("bird\tpre_acc\tpre_n\tpost_acc\tpost_n\tdelta\n")
        for bird, p1, n1, p2, n2, d in sorted(bird_rows):
            f.write(f"{bird}\t{p1}\t{n1}\t{p2}\t{n2}\t{d}\n")

    conf_items = post_items_all
    birds = sorted({r["questionedBird"] for r in conf_items} | {r["givenAnswer"] for r in conf_items})
    idx = {b: i for i, b in enumerate(birds)}
    mat = np.zeros((len(birds), len(birds)), dtype=int)

    for item in conf_items:
        if item["correct"] == "True":
            continue
        mat[idx[item["questionedBird"]], idx[item["givenAnswer"]]] += 1

    plt.figure(figsize=(10, 8))
    plt.imshow(mat)
    plt.title("Absolute Verwechslungsmatrix")
    plt.xlabel("Gewählte Vogelart")
    plt.ylabel("Tatsächliche Vogelart")
    plt.xticks(range(len(birds)), birds, rotation=90)
    plt.yticks(range(len(birds)), birds)
    plt.tight_layout()
    plt.savefig(os.path.join(OUT_DIR, "confusion_matrix_wrong_post.png"), dpi=200)
    plt.close()

    print("=== Häufigste Verwechslungen ===")
    conf_pairs = []
    for i, true_bird in enumerate(birds):
        for j, given_bird in enumerate(birds):
            if mat[i, j] > 0:
                conf_pairs.append((mat[i, j], true_bird, given_bird))
    for count, true_bird, given_bird in sorted(conf_pairs, reverse=True)[:10]:
        print(f"{true_bird} -> {given_bird}: {count}x")
    print()

    opportunity_counts = defaultdict(int)
    confusion_counts = defaultdict(int)
    items_with_options = [x for x in post_items_all if x["options"]]

    if items_with_options:
        birds_norm = sorted({x["questionedBird"] for x in items_with_options} | {x["givenAnswer"] for x in items_with_options})
        idx_norm = {b: i for i, b in enumerate(birds_norm)}
        norm_mat = np.zeros((len(birds_norm), len(birds_norm)), dtype=float)

        for item in items_with_options:
            true_bird = item["questionedBird"]
            chosen = item["givenAnswer"]

            for opt in item["options"]:
                if opt != true_bird:
                    opportunity_counts[(true_bird, opt)] += 1

            if chosen != true_bird:
                confusion_counts[(true_bird, chosen)] += 1

        for (true_bird, wrong_bird), opp in opportunity_counts.items():
            conf = confusion_counts.get((true_bird, wrong_bird), 0)
            norm_mat[idx_norm[true_bird], idx_norm[wrong_bird]] = conf / opp if opp > 0 else 0.0

        np.fill_diagonal(norm_mat, 0.0)

        plt.figure(figsize=(10, 8))
        sns.heatmap(
            norm_mat,
            xticklabels=birds_norm,
            yticklabels=birds_norm,
            cmap="Reds",
            annot=True,
            fmt=".2f",
            linewidths=0.5
        )
        plt.title("Normalisierte Verwechslungsrate")
        plt.xlabel("Gewählte Vogelart")
        plt.ylabel("Tatsächliche Vogelart")
        plt.tight_layout()
        plt.savefig(os.path.join(OUT_DIR, "normalized_confusion_matrix.png"), dpi=300)
        plt.close()

        print("=== Top normalisierte Verwechslungen ===")
        norm_pairs = []
        for (true_bird, wrong_bird), opp in opportunity_counts.items():
            conf = confusion_counts.get((true_bird, wrong_bird), 0)
            if opp > 0 and conf > 0:
                norm_pairs.append((conf / opp, conf, opp, true_bird, wrong_bird))

        for rate, conf, opp, true_bird, wrong_bird in sorted(norm_pairs, reverse=True)[:10]:
            print(f"{true_bird} -> {wrong_bird}: {rate:.2%} ({conf}/{opp})")
        print()

    print(f"Fertig. Ergebnisse gespeichert in: {OUT_DIR}")

if __name__ == "__main__":
    main()