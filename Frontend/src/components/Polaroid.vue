<script setup lang="ts">
const { c1 = '#C9A27A', c2 = '#7A4E2F', dish = 'untitled', glyph = '◐', size = 88, rotate = 0, src } =
  defineProps<{ c1?: string; c2?: string; dish?: string; glyph?: string; size?: number; rotate?: number; src?: string }>()
</script>

<template>
  <div
    class="shrink-0 flex flex-col bg-paper shadow-polaroid"
    :style="{
      width: `${size}px`,
      height: `${size + 22}px`,
      padding: '5px',
      paddingBottom: 0,
      transform: `rotate(${rotate}deg)`,
    }"
  >
    <div class="relative flex-1 overflow-hidden">
      <!-- Photo mode -->
      <img v-if="src" :src="src" class="absolute inset-0 w-full h-full object-cover" />
      <!-- Gradient mode -->
      <template v-else>
        <div
          class="absolute inset-0"
          :style="{ background: `linear-gradient(135deg, ${c1} 0%, ${c2} 100%)` }"
        />
        <div class="absolute inset-0" style="background-image: repeating-linear-gradient(135deg, transparent 0 6px, rgba(255,255,255,0.06) 6px 7px)" />
        <div
          class="absolute inset-0 flex items-center justify-center font-display italic text-paper/85"
          :style="{ fontSize: `${size * 0.42}px` }"
        >{{ glyph }}</div>
      </template>
    </div>
    <!-- caption strip -->
    <div
      class="font-mono text-cocoa-500 text-center uppercase truncate"
      style="padding: 4px 2px 0; height: 18px; font-size: 8px; letter-spacing: 0.3px;"
    >{{ dish }}</div>
  </div>
</template>
