import { defineConfig } from '@playwright/test';

// The QA site runs on a 2 GB server shared with production, so tests run one at
// a time. Nothing is recorded (no traces, screenshots or video): QA's database
// is a copy of production and holds real names, which must not end up in files
// attached to a public repository's build.
export default defineConfig({
  testDir: './specs',
  fullyParallel: false,
  workers: 1,
  retries: 0,
  timeout: 90_000,
  expect: { timeout: 15_000 },
  reporter: [['list']],
  use: {
    baseURL: process.env.E2E_BASE_URL ?? 'http://qa.reports.methodfit.net',
    trace: 'off',
    screenshot: 'off',
    video: 'off',
    actionTimeout: 20_000,
    navigationTimeout: 60_000,
  },
});
