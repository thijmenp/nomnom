<script setup lang="ts">
type Tab = 'feed' | 'map' | 'add' | 'lists' | 'profile'

const activeTab = defineModel<Tab>({ default: 'feed' })

const tabs = [
  { id: 'feed'    as Tab, label: 'Journal', icon: 'M2 4h12M2 8h12M2 12h8' },
  { id: 'map'     as Tab, label: 'Atlas',   icon: 'M2 3l4-1 4 1 4-1v12l-4 1-4-1-4 1V3z M6 2v12 M10 3v12' },
  { id: 'add'     as Tab, label: 'Log',     icon: 'M8 3v10 M3 8h10' },
  { id: 'lists'   as Tab, label: 'Lists',   icon: 'M3 4h10 M3 8h10 M3 12h6' },
  { id: 'profile' as Tab, label: 'You',     icon: 'M8 8a3 3 0 100-6 3 3 0 000 6z M2 14c0-3 3-5 6-5s6 2 6 5' },
]

const navTabs = tabs.filter(t => t.id !== 'add')
</script>

<template>
  <div class="flex h-screen bg-paper overflow-hidden">

    <!-- ── Sidebar (lg+) ──────────────────────────────────── -->
    <aside class="hidden lg:flex flex-col w-[220px] shrink-0 border-r border-rule px-6 py-8 gap-6 overflow-y-auto">

      <div>
        <div class="caption mb-1.5">Vol. III · No. 17</div>
        <div class="font-display text-[36px] leading-[0.95] text-ink mt-1.5">
          Nom<em>Nom</em>
        </div>
      </div>

      <nav class="flex flex-col gap-1">
        <button
          v-for="item in navTabs"
          :key="item.id"
          @click="activeTab = item.id"
          class="px-3 py-2 text-left font-display italic text-[17px] rounded-[4px] transition-colors cursor-pointer border-none"
          :class="activeTab === item.id
            ? 'bg-ink text-paper'
            : 'text-ink hover:bg-paper-warm'"
        >
          {{ item.label }}
        </button>
      </nav>

      <button @click="activeTab = 'add'" class="btn btn--accent self-start">
        + New entry
      </button>

      <div class="mt-auto">
        <div class="field-label mb-2">This year</div>
        <div class="grid grid-cols-2 gap-2">
          <div>
            <div class="font-display italic text-[28px] text-ink leading-none">147</div>
            <div class="caption mt-1">entries</div>
          </div>
          <div>
            <div class="font-display italic text-[28px] text-ink leading-none">8.4</div>
            <div class="caption mt-1">avg rating</div>
          </div>
        </div>
      </div>
    </aside>

    <!-- ── Main + mobile tab bar ──────────────────────────── -->
    <div class="flex-1 flex flex-col min-w-0 overflow-hidden">
      <main class="flex-1 overflow-y-auto pb-tab-bar lg:pb-0">
        <slot />
      </main>

      <!-- Mobile tab bar (hidden lg+) -->
      <nav
        class="lg:hidden fixed bottom-0 left-0 right-0 z-40 flex justify-around items-end pt-2.5 pb-7"
        style="background: linear-gradient(180deg, rgba(251,247,238,0) 0%, rgba(251,247,238,0.95) 30%, #FBF7EE 70%); border-top: 1px solid rgba(60,40,20,0.06);"
      >
        <button
          v-for="tab in tabs"
          :key="tab.id"
          @click="activeTab = tab.id"
          class="flex flex-col items-center gap-[3px] px-2.5 py-1 bg-transparent border-none cursor-pointer"
        >
          <!-- Log: circular dark button -->
          <div
            v-if="tab.id === 'add'"
            class="w-[38px] h-[38px] rounded-full bg-ink flex items-center justify-center shadow-fab mb-[1px]"
          >
            <svg width="14" height="14" viewBox="0 0 16 16" fill="none">
              <path :d="tab.icon" stroke="#FBF7EE" stroke-width="1.8" stroke-linecap="round"/>
            </svg>
          </div>

          <!-- Regular tabs -->
          <svg v-else width="20" height="20" viewBox="0 0 16 16" fill="none">
            <path
              :d="tab.icon"
              :stroke="activeTab === tab.id ? '#2A1F18' : '#A89680'"
              stroke-width="1.4" stroke-linecap="round" stroke-linejoin="round" fill="none"
            />
          </svg>

          <span
            class="font-mono text-[9px] tracking-[0.5px] uppercase"
            :class="activeTab === tab.id ? 'text-ink' : 'text-cocoa-400'"
          >{{ tab.label }}</span>
        </button>
      </nav>
    </div>

  </div>
</template>
