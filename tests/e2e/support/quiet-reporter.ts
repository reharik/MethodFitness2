import type { Reporter, TestCase, TestResult, FullResult } from '@playwright/test/reporter';

// Prints results without the page content Playwright normally includes in a
// failure. QA's database holds real names, and a build log on a public
// repository is world-readable. Playwright's own messages quote the elements a
// locator matched ("resolved to <div>…</div> aka getByText('…')"), which can
// carry a person's name, so only the lines that describe the assertion are kept.
const KEEP = /^(Error:|Expected|Received|Locator:|Timeout|Call log:|\s+- (Expect|waiting for locator|locator resolved to \d+ elements?))/;
const DROP = /aka |resolved to <|<\w+[\s>]|unexpected value "/;

function safeLines(message: string): string[] {
  return message
    .replace(/\u001b\[[0-9;]*m/g, '')
    .split('\n')
    .filter((line) => KEEP.test(line) && !DROP.test(line))
    .map((line) => line.slice(0, 300));
}

export default class QuietReporter implements Reporter {
  private counts: Record<string, number> = {};

  onStdOut(chunk: string | Buffer): void { process.stdout.write(chunk); }
  onStdErr(chunk: string | Buffer): void { process.stderr.write(chunk); }

  onTestEnd(test: TestCase, result: TestResult): void {
    this.counts[result.status] = (this.counts[result.status] ?? 0) + 1;
    const mark = { passed: '✓', failed: '✘', timedOut: '✘', skipped: '-', interrupted: '!' }[result.status] ?? '?';
    const where = `${test.location.file.split('/').slice(-2).join('/')}:${test.location.line}`;
    console.log(`  ${mark}  ${where} › ${test.titlePath().slice(3).join(' › ')} (${(result.duration / 1000).toFixed(1)}s)`);
    if (result.status === 'skipped') {
      const why = test.annotations.find((a) => a.type === 'skip')?.description;
      if (why) console.log(`       skipped: ${why}`);
    }
    for (const error of result.errors) {
      const at = error.location ? ` at ${error.location.file.split('/').slice(-2).join('/')}:${error.location.line}` : '';
      const lines = safeLines(error.message ?? '');
      console.log(`       failed${at}`);
      for (const line of lines.length ? lines : ['(details withheld: the message quoted page content)']) console.log(`       ${line}`);
    }
  }

  onEnd(result: FullResult): void {
    console.log(`\n  ${Object.entries(this.counts).map(([k, v]) => `${v} ${k}`).join(', ')} — overall: ${result.status}`);
  }
}
