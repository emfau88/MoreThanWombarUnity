# B8 session review — 9 October 2026

## Actual UI and input

- Current Unity UI viewed at 1280×600: `b8-home.png`, `b8-controls.png`, `b8-options.png`, `b8-intro.png`, `b8-intro-touch.png`, `b8-pause-touch.png`, `b8-defeat.png` under ignored `UnityProject/Assets/QA/`.
- Intro Primary button advanced to page 3; Finish/skip resumes the real chapter. The review restored the prior IntroSeen preference so it does not suppress the user's introduction.
- Two focused PlayMode session cases passed: `b8-session-results.json`. They cover a real attack and held virtual controls during pause, physical-gamepad-shaped Start input, saved options, actual player-death receiver and the UI Retry button, with time/audio cleanup on scene unload.
- UI uses existing Unity Canvas/Text/Button/Slider/EventSystem/InputSystemUIInputModule. No new UI package or external art download. Scene adds one ChapterSession and changes only header/title sizing; B7 world and gameplay values remain.

## Actual combat and retry

One unchanged motor/Combo bot run, without teleports or direct damage calls, cleared Anlieferung and opened the switch gate, then lost in Sortierhof: 1 completed area, 0 HP, 44.2 s wall time. It used the existing simple Light strategy; it is not evidence of a complete combat victory or a diagnosis that B8 changed balance. Original report: `b8-combat-run.txt`.

Pause/resume was exercised during that actual run. After death, the review's editor pause was released for the next session frame: Page Result, scaled time frozen, actual defeat UI shown. Its AB CHECKPOINT button restored Fighting in area index 1, saved entry HP 42, completed areas 1 and opened gate true. No health override was used for that retry.

## Separate victory/progression evidence

`b8-victory-flow.png` shows the real result UI after the existing scripted full-route progression case. That case disables brains and calls real enemy damage/death receivers deliberately; it checks all nine waves, gates, result state, frozen time and NOCHMAL SPIELEN → fresh Arrival. It is not another actual combat victory. Its original helper waited on scaled time after the result paused the game and left other brains active while processing one target; these fixture assumptions were corrected to realtime waiting and disabling the whole wave before scripted damage. Only this affected case was repeated.

## Capture and tool limits

The batch Editor used Direct3D11. CaptureHud temporarily renders current overlay canvases through the camera, calculates responsive scale from the requested target resolution, and bypasses camera postprocessing for UI. It restores camera aspect/target, postprocessing, Canvas mode/camera/distance/scale and CanvasScaler enabled state. These are current UI/layout views, not physical Android/browser/controller performance proofs.

The initial GUI Editor was blocked by Recovering Scene Backups. Computer Use read the dialog but input failed with GetCursorPos access denied. `Temp/__Backupscenes/0.backup` was copied and hash-verified as ignored `Assets/_Recovery/B7-preserved-before-B8.unity`; only the still-starting project Editor was closed and relaunched in batch mode. The source chapter was not replaced by its backup. No broad regression/build/install/publish was performed.

Final isolated full-route result: 1/1 passed in 39.87 s (`b8-chapter-flow-results.json`), in addition to the 2/2 session cases. The corrected test reaches Complete and exercises the actual replay Button, with fresh Arrival/HP/gates afterward.

Final handoff: batch Editor closed gracefully, normal GUI Editor reopened with saved JunkyardChapter; scene dirty false, Play false, compiling false, scriptCompilationFailed false, timeScale 1 and AudioListener.pause false. Imported material serialization noise was restored to the B7 source; no material content diff remains. Review inputs/staged UI states are not saved in the chapter.
