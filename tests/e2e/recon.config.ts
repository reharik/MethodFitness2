import { defineConfig } from '@playwright/test';
import base from './playwright.config';

// One-off exploration runs: prints the structure of each screen to the log.
export default defineConfig({ ...base, testDir: './recon', timeout: 300_000 });
