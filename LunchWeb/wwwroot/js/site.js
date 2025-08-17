console.log('site.js LOADED');

(function () {
    const form = document.querySelector('form[asp-action="Save"], form[action*="/Save"]');
    if (!form) return;

    const MODE = "delta";

    const selects = form.querySelectorAll('select.form-select');
    const totalEl = document.getElementById('pf-total-value');
    if (!totalEl) return;

    function parsePriceFromOption(opt) {
        if (!opt) return 0;
        if (opt.dataset && opt.dataset.price) {
            return parseFloat(String(opt.dataset.price).replace(',', '.')) || 0;
        }
        const m = (opt.textContent || "").match(/([-+]?\d+(?:[.,]\d+)?)/);
        return m ? parseFloat(m[1].replace(',', '.')) : 0;
    }

    selects.forEach(sel => {
        const initOpt = sel.selectedOptions && sel.selectedOptions[0];
        const initPrice = initOpt && initOpt.value ? parsePriceFromOption(initOpt) : 0;
        sel.dataset.initialPrice = String(initPrice);
    });

    function updateTotal() {
        let sum = 0;

        selects.forEach(sel => {
            const curOpt = sel.selectedOptions && sel.selectedOptions[0];
            const curPrice = (curOpt && curOpt.value) ? parsePriceFromOption(curOpt) : 0;
            const initPrice = parseFloat(sel.dataset.initialPrice || "0");

            if (MODE === "delta") {
                sum += (curPrice - initPrice);
            } else {
                if (initPrice === 0) sum += curPrice;
            }
        });

        const sign = sum > 0 ? "" : (sum < 0 ? "−" : "");
        const abs = Math.abs(sum).toFixed(2).replace('.', ',');
        totalEl.textContent = `${sign}${abs}`;
    }

    selects.forEach(s => s.addEventListener('change', updateTotal));
    updateTotal();
})();

(() => {
    document.addEventListener('click', async (e) => {
        const btn = e.target.closest('[data-detail-btn="true"]');
        if (!btn) return;

        const cell = btn.closest('td');
        const select = cell?.querySelector('[data-offer-select="true"]');
        const offerId = select?.value;
        const week = btn.dataset.week;

        const modalHostEl = document.getElementById('modalHost');
        if (!modalHostEl) {
            console.error('Chybí #modalHost ve stránce.');
            return;
        }
        const detailUrl = modalHostEl.dataset.mealDetailUrl;
        if (!detailUrl) {
            console.error('Chybí data-meal-detail-url na #modalHost.');
            return;
        }

        const showModal = (html) => {
            modalHostEl.innerHTML = html;
            const modalEl = document.getElementById('mealDetailModal');
            if (!modalEl) return;
            new bootstrap.Modal(modalEl).show();
        };

        if (!offerId) {
            showModal(`
        <div class="modal fade" id="mealDetailModal" tabindex="-1" aria-hidden="true">
          <div class="modal-dialog"><div class="modal-content">
            <div class="modal-header"><h5 class="modal-title">Detail jídla</h5>
              <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body"><div class="alert alert-info mb-0">Nejprve vyberte jídlo v seznamu.</div></div>
          </div></div>
        </div>
      `);
            return;
        }

        try {
            const url = `${detailUrl}?dailyOfferId=${encodeURIComponent(offerId)}&selectedWeek=${encodeURIComponent(week)}`;
            const resp = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
            const html = await resp.text();

            if (!resp.ok || !html.includes('id="mealDetailModal"')) {
                showModal(`
          <div class="modal fade" id="mealDetailModal" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog"><div class="modal-content">
              <div class="modal-header"><h5 class="modal-title">Chyba</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
              </div>
              <div class="modal-body"><div class="alert alert-danger mb-0">${html || 'Nepodařilo se načíst detail jídla.'}</div></div>
            </div></div>
          </div>
        `);
                return;
            }

            showModal(html);
        } catch (err) {
            console.error(err);
            showModal(`
        <div class="modal fade" id="mealDetailModal" tabindex="-1" aria-hidden="true">
          <div class="modal-dialog"><div class="modal-content">
            <div class="modal-header"><h5 class="modal-title">Chyba</h5>
              <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body"><div class="alert alert-danger mb-0">Nepodařilo se načíst detail jídla.</div></div>
          </div></div>
        </div>
      `);
        }
    });

    document.addEventListener('change', (e) => {
        const select = e.target.closest('[data-offer-select="true"]');
        if (!select) return;

        const cell = select.closest('td');
        const btn = cell?.querySelector('[data-detail-btn="true"]');
        if (!btn) return;

        btn.disabled = !select.value;
    });

    window.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('[data-offer-select="true"]').forEach((sel) => {
            const cell = sel.closest('td');
            const btn = cell?.querySelector('[data-detail-btn="true"]');
            if (btn) btn.disabled = !sel.value;
        });
    });
})();
