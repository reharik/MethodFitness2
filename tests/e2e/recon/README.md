Exploration specs live here while tests are being written. They print the
structure of a screen (controls and labels, never data) so a test can be written
against what the app actually renders. Run with `npm run recon`, or in CI by
adding an empty `tests/e2e/.run-recon` file. The folder is normally empty.
