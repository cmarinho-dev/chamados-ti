(() => {
  const listener = document.querySelector("[data-admin-listener]");
  if (!listener) return;

  const latestIdEl = document.querySelector("[data-latest-id-value]");
  const totalOpenEl = document.querySelector("[data-total-open]");
  const audioButton = document.querySelector("[data-audio-enable]");
  const notifySound = document.getElementById("notifySound");

  let latestId = Number(listener.dataset.latestId || 0);
  let audioUnlocked = false;

  const audioPrefKey = "chamadosTI.audioEnabled";
  const playAfterReloadKey = "chamadosTI.playAfterReload";

  /* ---------------- UI DO BOTÃO DE SOM ---------------- */

  const setAudioUi = (enabled) => {
    if (!audioButton) return;
    audioButton.textContent = enabled ? "Som ativado" : "Ativar som";
  };

  /* ---------------- DESBLOQUEAR ÁUDIO ---------------- */

  const tryUnlockAudio = () => {
    if (!notifySound) {
      audioUnlocked = true;
      return;
    }

    notifySound.currentTime = 0;

    notifySound.play()
      .then(() => {
        notifySound.pause();
        notifySound.currentTime = 0;
        audioUnlocked = true;
      })
      .catch(() => {
        audioUnlocked = false;
      });
  };

  /* ---------------- ATIVAR SOM ---------------- */

  const requestEnableAudio = () => {
    localStorage.setItem(audioPrefKey, "true");
    tryUnlockAudio();
    setAudioUi(true);
  };

  /* ---------------- PRIMEIRA INTERAÇÃO ---------------- */

  const handleFirstInteraction = () => {
    const wantsAudio = localStorage.getItem(audioPrefKey) === "true";
    if (!wantsAudio) return;

    tryUnlockAudio();
  };

  document.addEventListener("click", handleFirstInteraction);
  document.addEventListener("keydown", handleFirstInteraction);

  if (audioButton) {
    audioButton.addEventListener("click", requestEnableAudio);
  }

  /* ---------------- ESTADO INICIAL ---------------- */

  const wantsAudio = localStorage.getItem(audioPrefKey) === "true";

  if (wantsAudio) {
    setAudioUi(true);
    tryUnlockAudio();
  } else {
    setAudioUi(false);
  }

  /* ---------------- TOCAR SOM ---------------- */

  const playPing = () => {
    const wantsAudio = localStorage.getItem(audioPrefKey) === "true";

    if (!wantsAudio) return;
    if (!notifySound) return;

    if (!audioUnlocked) {
      tryUnlockAudio();
      if (!audioUnlocked) return;
    }

    notifySound.currentTime = 0;
    notifySound.play().catch(() => {});
  };

  /* ---------------- SOM APÓS RELOAD ---------------- */

  document.addEventListener("DOMContentLoaded", () => {
    const shouldPlay = sessionStorage.getItem(playAfterReloadKey);

    if (shouldPlay === "true") {
      sessionStorage.removeItem(playAfterReloadKey);

      setTimeout(() => {
        playPing();
      }, 200);
    }
  });

  /* ---------------- VERIFICAR ATUALIZAÇÕES ---------------- */

  const checkUpdates = async () => {
    try {
      const response = await fetch("/admin/updates", {
        headers: { "Accept": "application/json" }
      });

      if (!response.ok) return;

      const data = await response.json();

      if (typeof data.latestId === "number" && data.latestId > latestId) {

        // marca para tocar som após reload
        const wantsAudio = localStorage.getItem(audioPrefKey) === "true";

        if (wantsAudio) {
          sessionStorage.setItem(playAfterReloadKey, "true");
        }

        window.location.reload();
        return;
      }

      if (typeof data.latestId === "number") {
        latestId = data.latestId;
        listener.dataset.latestId = String(data.latestId);

        if (latestIdEl) {
          latestIdEl.textContent = String(data.latestId);
        }
      }

      if (typeof data.totalOpen === "number" && totalOpenEl) {
        totalOpenEl.textContent = String(data.totalOpen);
      }

    } catch {
      // ignora erro de rede
    }
  };

  /* ---------------- POLLING ---------------- */

  setInterval(checkUpdates, 5000);

  /* ---------------- ATUALIZAÇÃO DE STATUS ---------------- */

  const statusOrder = ["Aberto", "Em andamento", "Finalizado"];

  const tokenInput = document.querySelector(
    ".js-status-token input[name='__RequestVerificationToken']"
  );

  const obterProximoStatus = (atual) => {
    const idx = statusOrder.indexOf(atual);
    if (idx === -1) return statusOrder[0];

    return statusOrder[(idx + 1) % statusOrder.length];
  };

  const atualizarStatus = async (id, status) => {

    const formData = new FormData();
    formData.append("id", id);
    formData.append("situacao", status);

    if (tokenInput) {
      formData.append("__RequestVerificationToken", tokenInput.value);
    }

    const response = await fetch("/admin/atualizar-situacao", {
      method: "POST",
      body: formData
    });

    return response.ok;
  };


  document.addEventListener("click", async (event) => {
    const ticket = event.target.closest(".ticket-clickable");
    if (!ticket) return;

    const id = ticket.dataset.ticketId;
    const atual = ticket.dataset.status || "Aberto";
    const proximo = obterProximoStatus(atual);

    const ok = await atualizarStatus(id, proximo);
    if (!ok) return;

    ticket.dataset.status = proximo;
    const pill = ticket.querySelector(".pill.status");
    if (pill) {
      pill.textContent = proximo;
      pill.className = `pill status status-${proximo.replace(" ", "-").toLowerCase()}`;
    }
  });
})();

(() => {
  const form = document.querySelector("[data-open-ticket-form]");
  if (!form) return;

  const nameInput = form.querySelector("[data-first-field]");
  const setorSelect = form.querySelector("[data-next-field]");
  const placeholderSelect = form.querySelector(".js-placeholder-select");
  const storageKey = "chamadosTI.openTicketDraft";

  const syncSelectPlaceholder = () => {
    if (!placeholderSelect) return;
    const hasValue = String(placeholderSelect.value || "").trim().length > 0;
    placeholderSelect.classList.toggle("is-placeholder", !hasValue);
  };

  const loadDraft = () => {
    try {
      const raw = window.localStorage.getItem(storageKey);
      if (!raw) return;
      const data = JSON.parse(raw);
      if (data && typeof data === "object") {
        if (nameInput && typeof data.nome === "string" && data.nome.trim()) {
          nameInput.value = data.nome;
        }
        if (setorSelect && typeof data.setor === "string" && data.setor.trim()) {
          setorSelect.value = data.setor;
        }
      }
    } catch {
      // ignore storage errors
    }
  };

  const saveDraft = () => {
    try {
      const payload = {
        nome: nameInput ? nameInput.value.trim() : "",
        setor: setorSelect ? setorSelect.value : ""
      };
      window.localStorage.setItem(storageKey, JSON.stringify(payload));
    } catch {
      // ignore storage errors
    }
  };

  loadDraft();
  syncSelectPlaceholder();
  if (placeholderSelect) {
    placeholderSelect.addEventListener("change", syncSelectPlaceholder);
  }
  if (nameInput) {
    nameInput.addEventListener("input", saveDraft);
  }
  if (setorSelect) {
    setorSelect.addEventListener("change", saveDraft);
  }

  form.addEventListener("submit", saveDraft);

  form.addEventListener("keydown", (event) => {
    if (event.key !== "Enter") return;

    if (event.target === nameInput) {
      event.preventDefault();
      if (setorSelect) {
        setorSelect.focus();
      }
      return;
    }

    if (event.target === setorSelect) {
      event.preventDefault();
      if (typeof form.requestSubmit === "function") {
        form.requestSubmit();
      } else {
        form.submit();
      }
    }
  });
})();

(() => {
  const form = document.querySelector("[data-inventario-form]");
  if (!form) return;

  const updateSelectPlaceholder = (select) => {
    const hasValue = String(select.value || "").trim().length > 0;
    select.classList.toggle("is-placeholder", !hasValue);
  };

  form.querySelectorAll(".js-placeholder-select").forEach((select) => {
    updateSelectPlaceholder(select);
    select.addEventListener("change", () => updateSelectPlaceholder(select));
  });

  const reindexTextRepeater = (repeater) => {
    const prefix = repeater.dataset.prefix;
    const items = repeater.querySelectorAll("[data-item]");
    items.forEach((item, index) => {
      const input = item.querySelector("input");
      if (!input) return;
      input.name = `${prefix}[${index}].Descricao`;
    });
  };

  const ensureAtLeastOneTextItem = (repeater) => {
    const itemsContainer = repeater.querySelector(".repeater-items");
    if (!itemsContainer) return;
    const hasItems = itemsContainer.querySelectorAll("[data-item]").length > 0;
    if (hasItems) return;

    const row = document.createElement("div");
    row.className = "repeater-item";
    row.setAttribute("data-item", "true");
    row.innerHTML = `
      <input class="input" maxlength="120" />
      <button class="btn ghost icon-btn" type="button" data-remove-item>Remover</button>
    `;
    itemsContainer.appendChild(row);
    reindexTextRepeater(repeater);
  };

  form.querySelectorAll("[data-repeater]").forEach((repeater) => {
    const itemsContainer = repeater.querySelector(".repeater-items");
    const addButton = repeater.querySelector("[data-add-item]");
    if (!itemsContainer || !addButton) return;

    addButton.addEventListener("click", () => {
      const row = document.createElement("div");
      row.className = "repeater-item";
      row.setAttribute("data-item", "true");
      row.innerHTML = `
        <input class="input" maxlength="120" />
        <button class="btn ghost icon-btn" type="button" data-remove-item>Remover</button>
      `;
      itemsContainer.appendChild(row);
      reindexTextRepeater(repeater);
      const input = row.querySelector("input");
      if (input) input.focus();
    });

    itemsContainer.addEventListener("click", (event) => {
      const btn = event.target.closest("[data-remove-item]");
      if (!btn) return;
      const row = btn.closest("[data-item]");
      if (!row) return;
      row.remove();
      ensureAtLeastOneTextItem(repeater);
      reindexTextRepeater(repeater);
    });

    reindexTextRepeater(repeater);
  });

  const monitorRepeater = form.querySelector("[data-monitor-repeater]");
  if (!monitorRepeater) return;

  const monitorContainer = monitorRepeater.querySelector(".repeater-items");
  const addMonitorButton = monitorRepeater.querySelector("[data-add-monitor]");
  if (!monitorContainer || !addMonitorButton) return;

  const reindexMonitorRows = () => {
    const rows = monitorContainer.querySelectorAll("[data-monitor-item]");
    rows.forEach((row, index) => {
      const fields = row.querySelectorAll("input");
      if (fields.length !== 3) return;
      fields[0].name = `Monitores[${index}].InventarioNumero`;
      fields[1].name = `Monitores[${index}].Marca`;
      fields[2].name = `Monitores[${index}].Polegadas`;
    });
  };

  const ensureAtLeastOneMonitor = () => {
    const rows = monitorContainer.querySelectorAll("[data-monitor-item]");
    if (rows.length > 0) return;

    const row = document.createElement("div");
    row.className = "repeater-item monitor-item";
    row.setAttribute("data-monitor-item", "true");
    row.innerHTML = `
      <input class="input" placeholder="N° inventário" maxlength="30" />
      <input class="input" placeholder="Marca" maxlength="60" />
      <input class="input" placeholder="Polegadas" maxlength="10" />
      <button class="btn ghost icon-btn" type="button" data-remove-monitor>Remover</button>
    `;
    monitorContainer.appendChild(row);
  };

  addMonitorButton.addEventListener("click", () => {
    const row = document.createElement("div");
    row.className = "repeater-item monitor-item";
    row.setAttribute("data-monitor-item", "true");
    row.innerHTML = `
      <input class="input" placeholder="N° inventário" maxlength="30" />
      <input class="input" placeholder="Marca" maxlength="60" />
      <input class="input" placeholder="Polegadas" maxlength="10" />
      <button class="btn ghost icon-btn" type="button" data-remove-monitor>Remover</button>
    `;
    monitorContainer.appendChild(row);
    reindexMonitorRows();
    const firstInput = row.querySelector("input");
    if (firstInput) firstInput.focus();
  });

  monitorContainer.addEventListener("click", (event) => {
    const button = event.target.closest("[data-remove-monitor]");
    if (!button) return;
    const row = button.closest("[data-monitor-item]");
    if (!row) return;
    row.remove();
    ensureAtLeastOneMonitor();
    reindexMonitorRows();
  });

  ensureAtLeastOneMonitor();
  reindexMonitorRows();
})();

(() => {
  const links = document.querySelectorAll(".sidebar-link");
  if (!links.length) return;

  const path = window.location.pathname.toLowerCase();
  links.forEach((link) => {
    const href = (link.getAttribute("href") || "").toLowerCase();
    if (!href) return;
    if (path === href || (href !== "/admin" && path.startsWith(href))) {
      link.classList.add("active");
    }
  });
})();

(() => {
  const nav = document.querySelector("[data-admin-nav]");
  if (!nav) return;

  const toggle = nav.querySelector("[data-admin-nav-toggle]");
  const menu = nav.querySelector("[data-admin-nav-menu]");
  const dropdownToggles = nav.querySelectorAll("[data-admin-dropdown-toggle]");
  const dropdownContainers = nav.querySelectorAll(".admin-nav-dropdown");

  if (toggle && menu) {
    toggle.addEventListener("click", () => {
      const expanded = toggle.getAttribute("aria-expanded") === "true";
      toggle.setAttribute("aria-expanded", String(!expanded));
      menu.classList.toggle("open", !expanded);
    });
  }

  if (dropdownToggles.length) {
    dropdownToggles.forEach((dropdownToggle) => {
      dropdownToggle.addEventListener("click", () => {
        const dropdownContainer = dropdownToggle.closest(".admin-nav-dropdown");
        if (!dropdownContainer) return;

        const expanded = dropdownToggle.getAttribute("aria-expanded") === "true";
        dropdownToggle.setAttribute("aria-expanded", String(!expanded));
        dropdownContainer.classList.toggle("open", !expanded);
      });
    });
  }

  document.addEventListener("click", (event) => {
    const target = event.target;
    if (!(target instanceof Element)) return;

    dropdownContainers.forEach((dropdownContainer) => {
      if (dropdownContainer.contains(target)) return;
      dropdownContainer.classList.remove("open");
      const toggle = dropdownContainer.querySelector("[data-admin-dropdown-toggle]");
      if (toggle) {
        toggle.setAttribute("aria-expanded", "false");
      }
    });
  });
})();

(() => {
  const searchInputs = document.querySelectorAll("[data-picker-search]");
  if (!searchInputs.length) return;

  searchInputs.forEach((input) => {
    const selector = input.getAttribute("data-picker-search");
    if (!selector) return;
    const container = document.querySelector(selector);
    if (!container) return;

    input.addEventListener("input", () => {
      const term = String(input.value || "").trim().toLowerCase();
      container.querySelectorAll(".picker-item").forEach((item) => {
        const text = (item.getAttribute("data-search-text") || item.textContent || "").toLowerCase();
        item.style.display = term.length === 0 || text.includes(term) ? "" : "none";
      });
    });
  });
})();

(() => {
  const chipsContainers = document.querySelectorAll("[data-picker-chips]");
  if (!chipsContainers.length) return;

  chipsContainers.forEach((chipsContainer) => {
    const selector = chipsContainer.getAttribute("data-picker-chips");
    if (!selector) return;

    const picker = document.querySelector(selector);
    if (!picker) return;

    const render = () => {
      chipsContainer.innerHTML = "";
      const selected = picker.querySelectorAll("input[type='checkbox']:checked");

      selected.forEach((input) => {
        const label = input.closest(".picker-item");
        if (!label) return;

        const text = (label.querySelector("span")?.textContent || "").trim();
        const chip = document.createElement("button");
        chip.type = "button";
        chip.className = "picker-chip";
        chip.innerHTML = `<span>${text}</span><strong>×</strong>`;
        chip.addEventListener("click", () => {
          input.checked = false;
          input.dispatchEvent(new Event("change", { bubbles: true }));
        });

        chipsContainer.appendChild(chip);
      });
    };

    picker.addEventListener("change", render);
    render();
  });
})();
