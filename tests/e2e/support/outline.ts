import type { Page } from '@playwright/test';

// Describes a screen's structure without its data. QA holds real client and
// trainer names, and build logs on a public repository are world-readable, so
// this prints controls, labels and menu text only: nothing from inside tables,
// grids, calendar events or dropdown options.
export async function outline(page: Page, scope = 'body'): Promise<string> {
  return page.evaluate((scopeSelector) => {
    const root = document.querySelector(scopeSelector) ?? document.body;
    const dataZones = 'table, tbody, .ui-jqgrid, .fc-event, .fc-event-container, option, .grid, [role="grid"], [role="row"]';
    const lines: string[] = [];
    const clip = (s: string | null | undefined, n = 60) => (s ?? '').replace(/\s+/g, ' ').trim().slice(0, n);
    const sel = (el: Element) => {
      const id = el.id ? `#${el.id}` : '';
      const cls = typeof el.className === 'string' && el.className.trim()
        ? '.' + el.className.trim().split(/\s+/).slice(0, 4).join('.') : '';
      return `${el.tagName.toLowerCase()}${id}${cls}`;
    };
    const visible = (el: Element) => {
      const r = (el as HTMLElement).getBoundingClientRect();
      return r.width > 0 && r.height > 0;
    };

    lines.push('-- regions');
    root.querySelectorAll('[id]').forEach((el) => {
      if (el.closest(dataZones) || !visible(el)) return;
      if (['DIV', 'SECTION', 'FORM', 'UL', 'NAV', 'TABLE'].includes(el.tagName)) lines.push('  ' + sel(el));
    });

    lines.push('-- fields');
    root.querySelectorAll('input, select, textarea').forEach((el) => {
      if (!visible(el)) return;
      const f = el as HTMLInputElement;
      const label = f.id ? document.querySelector(`label[for="${f.id}"]`) : null;
      lines.push(`  ${sel(el)} name=${f.name || '-'} type=${f.type || '-'} label="${clip(label?.textContent)}"`);
    });

    // Button captions are interface text. Link text is only printed inside the
    // menu: elsewhere a link can be a person's name (for example the client
    // tokens on a trainer's page), so only its selector and length are shown.
    lines.push('-- buttons');
    root.querySelectorAll('button, input[type=button], input[type=submit]').forEach((el) => {
      if (el.closest(dataZones) || !visible(el)) return;
      lines.push(`  ${sel(el)} text="${clip(el.textContent || (el as HTMLInputElement).value, 30)}"`);
    });
    lines.push('-- links');
    const linkKinds = new Map<string, number>();
    root.querySelectorAll('a').forEach((el) => {
      if (el.closest(dataZones) || !visible(el)) return;
      if (el.closest('#left-navigation')) { lines.push(`  ${sel(el)} menu="${clip(el.textContent, 30)}" rel=${el.getAttribute('rel') ?? '-'}`); return; }
      linkKinds.set(sel(el), (linkKinds.get(sel(el)) ?? 0) + 1);
    });
    linkKinds.forEach((n, k) => lines.push(`  ${k} x${n} (text withheld)`));

    lines.push('-- data containers (contents withheld)');
    root.querySelectorAll('table, .ui-jqgrid, .fc').forEach((el) => {
      if (!visible(el)) return;
      const heads = Array.from(el.querySelectorAll('th')).map((th) => clip(th.textContent, 25)).filter(Boolean);
      lines.push(`  ${sel(el)} rows=${el.querySelectorAll('tr').length} headers=[${heads.slice(0, 20).join(' | ')}]`);
    });

    return lines.join('\n');
  }, scope);
}
