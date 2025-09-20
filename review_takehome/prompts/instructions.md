You're reviewing a candidate for a senior Unity engineering role. The candidate has submitted a pull request for a take-home test.

🚦 Please perform a **critical and thorough code review** with the following goals:

---

### 🔍 What to Evaluate

1. **Feature Completeness**
   - Did the candidate implement *all required parts* of the Offline Practice Mode?
   - Are all core features — ghost saving/loading, UI display, and persistence — clearly covered?

2. **Architecture & Design**
   - Are components modular and extensible?
   - Is the logic decoupled from UI/input/persistence layers?

3. **Code Quality**
   - Clarity, naming, maintainability
   - Use of design patterns (State, Singleton, Strategy, etc.)
   - Adherence to Unity best practices (e.g., avoiding logic in `Update`, using coroutines where appropriate)

4. **Performance & Efficiency**
   - Is the system optimized for reuse and scalability?
   - Is any part overly complex for the scope?

5. **Testing & Debuggability**
   - Are there clear places for logging, debugging, or testing?

---

### ⚠️ Very Important

The diffs may appear as full deletions + re-additions due to tooling or formatting.  
**Do not assume the candidate wrote the whole file** — look for actual logic changes and implementation content.

---

### 📊 Output Format

Please give a score from **1–5** for each of the above areas, with a short rationale.  
End with a 1-paragraph **summary** and a clear recommendation:

- ✅ Move forward
- ⚠️ Weak/needs review
- ❌ Do not recommend

---

### 📦 Assignment Overview

**Feature: Offline Practice Mode with Ghost Racer**

Requirements:
- Add an "Offline Practice Mode" entry to the main menu
- Record and store the player's best race
- Display a ghost racer from the saved performance
- Show current lap/race time
- Persist ghost data across sessions
