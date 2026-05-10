<script setup lang="ts">
import Polaroid from './Polaroid.vue'

interface Collection {
  name: string
  subtitle: string
  count: number
  swatches: [string, string][]
}

const lists: Collection[] = [
  { name: 'Want to try',  subtitle: 'the queue',           count: 14, swatches: [['#7A2A22','#D9B26A'],['#6B8E7B','#2D3B33'],['#8C2F5F','#2A1A2C']] },
  { name: 'Going back',   subtitle: 'stamped & approved',  count: 22, swatches: [['#C9A27A','#7A4E2F'],['#E8C26C','#B07028'],['#7A2A22','#D9B26A']] },
  { name: 'Coffee crawl', subtitle: 'a Saturday',          count:  7, swatches: [['#C9A27A','#7A4E2F'],['#6B8E7B','#2D3B33'],['#D4B896','#8B6F4E']] },
  { name: 'For visitors', subtitle: 'when mom comes',      count:  9, swatches: [['#7A2A22','#D9B26A'],['#8C2F5F','#2A1A2C'],['#E8C26C','#B07028']] },
]
</script>

<template>
  <div class="pb-tab-bar lg:pb-12">

    <!-- ── Header ───────────────────────────────────────────── -->
    <div class="flex justify-between items-baseline px-gutter lg:px-9 pt-3.5 lg:pt-8 pb-2">
      <div>
        <div class="caption mb-1">Curated stacks</div>
        <div class="font-display italic text-[32px] text-ink leading-none mt-1">Collections</div>
      </div>
      <button class="font-display italic text-[22px] text-paprika bg-transparent border-none cursor-pointer leading-none">+</button>
    </div>

    <!-- ── Mobile: single column ────────────────────────────── -->
    <div class="lg:hidden px-gutter pt-3.5">
      <div
        v-for="(list, i) in lists" :key="list.name"
        class="flex items-center gap-3 py-4 cursor-pointer"
        :style="i < lists.length - 1 ? 'border-bottom: 0.5px dashed rgba(60,40,20,0.18)' : ''"
      >
        <!-- Polaroid stack thumbnail -->
        <div class="relative shrink-0" style="width:76px;height:76px">
          <div
            v-for="(sw, j) in list.swatches" :key="j"
            class="absolute"
            :style="`top:${j*2}px;left:${j*4}px;transform:rotate(${(j-1)*5}deg);z-index:${3-j}`"
          >
            <Polaroid :c1="sw[0]" :c2="sw[1]" dish="" :size="56" />
          </div>
        </div>
        <div class="flex-1 pl-2 min-w-0">
          <div class="font-display text-[22px] text-ink font-medium leading-tight">{{ list.name }}</div>
          <div class="font-display italic text-[13px] text-cocoa-500 mt-0.5">{{ list.subtitle }}</div>
          <div class="caption mt-1.5">{{ list.count }} entries</div>
        </div>
        <div class="font-display italic text-[22px] text-cocoa-400 shrink-0 leading-none">›</div>
      </div>
    </div>

    <!-- ── Desktop: two-column grid ─────────────────────────── -->
    <div class="hidden lg:grid grid-cols-2 gap-x-6 px-9 pt-3.5">
      <div
        v-for="list in lists" :key="list.name"
        class="flex items-center gap-3 py-5 cursor-pointer"
        style="border-bottom: 0.5px dashed rgba(60,40,20,0.18)"
      >
        <!-- Polaroid stack thumbnail -->
        <div class="relative shrink-0" style="width:76px;height:76px">
          <div
            v-for="(sw, j) in list.swatches" :key="j"
            class="absolute"
            :style="`top:${j*2}px;left:${j*4}px;transform:rotate(${(j-1)*5}deg);z-index:${3-j}`"
          >
            <Polaroid :c1="sw[0]" :c2="sw[1]" dish="" :size="56" />
          </div>
        </div>
        <div class="flex-1 pl-2 min-w-0">
          <div class="font-display text-[22px] text-ink font-medium leading-tight">{{ list.name }}</div>
          <div class="font-display italic text-[13px] text-cocoa-500 mt-0.5">{{ list.subtitle }}</div>
          <div class="caption mt-1.5">{{ list.count }} entries</div>
        </div>
        <div class="font-display italic text-[22px] text-cocoa-400 shrink-0 leading-none">›</div>
      </div>
    </div>

  </div>
</template>
