<script setup lang="ts">
const stats = [
  { label: 'Entries',     value: '147' },
  { label: 'Avg rating',  value: '8.4' },
  { label: 'Going back',  value: '63%' },
  { label: 'Cities',      value: '11'  },
]

const breakdown = [
  { glyph: '◔', label: 'Coffee', count: 62, avg: 8.6 },
  { glyph: '◑', label: 'Lunch',  count: 41, avg: 7.9 },
  { glyph: '◕', label: 'Dinner', count: 44, avg: 8.7 },
]

const neighborhoods = [
  { name: 'De Pijp',  visits: 28 },
  { name: 'Jordaan',  visits: 22 },
  { name: 'Centrum',  visits: 17 },
  { name: 'Oud-West', visits: 14 },
]

const maxCount = Math.max(...breakdown.map(b => b.count))
</script>

<template>
  <div class="pb-tab-bar lg:pb-12">

    <!-- ── Header ───────────────────────────────────────────── -->
    <div class="px-gutter lg:px-9 pt-3.5 lg:pt-8 pb-2">
      <div class="caption mb-1">The Editor</div>
      <div class="font-display text-[32px] text-ink leading-none mt-1">
        Your year, <em>so far</em>
      </div>
    </div>

    <!-- ── Two-column wrapper on desktop ────────────────────── -->
    <div class="lg:grid lg:grid-cols-2 lg:gap-x-12 lg:px-9 lg:pt-6">

      <!-- ── Left column: profile card + stats grid ─────────── -->
      <div>

        <!-- Profile card -->
        <div class="flex items-center gap-3.5 px-gutter lg:px-0 py-4 border-y border-rule lg:border-t-0">
          <div
            class="w-16 h-16 rounded-full shrink-0 flex items-center justify-center font-display italic text-[28px] text-paper"
            style="background: linear-gradient(135deg, #C9A27A, #7A4E2F)"
          >S</div>
          <div>
            <div class="font-display text-[22px] text-ink font-medium leading-tight">Sam Pieters</div>
            <div class="font-display italic text-[13px] text-cocoa-500 mt-0.5">
              logging since Mar 2024 · Amsterdam
            </div>
          </div>
        </div>

        <!-- Stats grid -->
        <div class="grid grid-cols-2 px-gutter lg:px-0">
          <div
            v-for="(stat, i) in stats" :key="stat.label"
            class="py-3.5"
            :class="[
              i % 2 === 0 ? 'pr-3.5 border-r border-rule' : 'pl-3.5',
              i < 2      ? 'border-b border-rule' : '',
            ]"
          >
            <div class="font-display italic text-[36px] text-ink font-medium leading-none">{{ stat.value }}</div>
            <div class="caption mt-1.5">{{ stat.label }}</div>
          </div>
        </div>

      </div>

      <!-- ── Right column: breakdown + neighborhoods ─────────── -->
      <div>

        <!-- By occasion -->
        <div class="px-gutter lg:px-0 pt-6 lg:pt-0">
          <div class="field-label mb-2">By occasion</div>
          <div class="flex flex-col gap-3.5">
            <div v-for="b in breakdown" :key="b.label">
              <div class="flex justify-between items-baseline mb-1">
                <div class="font-display italic text-[16px] text-ink">
                  <span class="mr-1.5">{{ b.glyph }}</span>{{ b.label }}
                </div>
                <div class="font-mono text-[11px] text-cocoa-700">
                  {{ b.count }} · avg {{ b.avg }}
                </div>
              </div>
              <div class="h-1.5 rounded-full bg-[rgba(60,40,20,0.08)] overflow-hidden">
                <div
                  class="h-full bg-ink rounded-full"
                  :style="{ width: `${(b.count / maxCount) * 100}%` }"
                />
              </div>
            </div>
          </div>
        </div>

        <!-- Top neighborhoods -->
        <div class="px-gutter lg:px-0 pt-6">
          <div class="field-label mb-2">Most-logged neighborhoods</div>
          <div class="flex flex-col gap-2.5">
            <div
              v-for="(n, i) in neighborhoods" :key="n.name"
              class="flex items-baseline gap-3 font-display"
            >
              <span class="font-mono text-[10px] text-cocoa-400 w-4 shrink-0">
                {{ String(i + 1).padStart(2, '0') }}
              </span>
              <span class="flex-1 text-[16px] text-ink">{{ n.name }}</span>
              <span class="italic text-[13px] text-cocoa-500">{{ n.visits }} visits</span>
            </div>
          </div>
        </div>

      </div>
    </div>

  </div>
</template>
