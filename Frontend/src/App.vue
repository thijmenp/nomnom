<script setup lang="ts">
import { ref } from 'vue'
import AppShell from './components/AppShell.vue'
import AddScreen from './components/AddScreen.vue'
import FeedScreen from './components/FeedScreen.vue'
import DetailScreen from './components/DetailScreen.vue'
import CollectionsScreen from './components/CollectionsScreen.vue'
import ProfileScreen from './components/ProfileScreen.vue'
import type { Spot } from './api/spots'

type Tab = 'feed' | 'map' | 'add' | 'lists' | 'profile'

const activeTab    = ref<Tab>('feed')
const selectedSpot = ref<Spot | null>(null)

const feedKey = ref(0)

function openSpot(spot: Spot) { selectedSpot.value = spot }
function closeSpot()          { selectedSpot.value = null }
function onDeleted()          { selectedSpot.value = null; feedKey.value++ }

const screens: Record<Tab, { title: string; sub: string }> = {
  feed:    { title: 'NomNom',       sub: 'a personal log of meals worth remembering' },
  map:     { title: 'Atlas',        sub: '52.36°N · 4.88°E' },
  add:     { title: 'New entry',    sub: 'log a meal' },
  lists:   { title: 'Collections',  sub: 'curated stacks' },
  profile: { title: 'Your year',    sub: 'so far' },
}
</script>

<template>
  <AppShell v-model="activeTab">
    <DetailScreen v-if="selectedSpot" :spot="selectedSpot" @back="closeSpot" @deleted="onDeleted" />
    <template v-else>
      <FeedScreen v-if="activeTab === 'feed'" :key="feedKey" @open="openSpot" />
      <AddScreen v-else-if="activeTab === 'add'" @cancel="activeTab = 'feed'" @saved="activeTab = 'feed'" />
      <CollectionsScreen v-else-if="activeTab === 'lists'" />
      <ProfileScreen v-else-if="activeTab === 'profile'" />
      <div v-else class="px-gutter pt-8">
        <p class="caption mb-2">{{ screens[activeTab].sub }}</p>
        <h1 class="font-display text-masthead text-ink italic leading-none">
          {{ screens[activeTab].title }}
        </h1>
        <hr class="rule--dash mt-6" />
      </div>
    </template>
  </AppShell>
</template>
