<script setup lang="ts">
import { ref, watch } from 'vue'
import AppShell from './components/AppShell.vue'
import AddScreen from './components/AddScreen.vue'
import FeedScreen from './components/FeedScreen.vue'
import DetailScreen from './components/DetailScreen.vue'
import CollectionsScreen from './components/CollectionsScreen.vue'
import CollectionDetailScreen from './components/CollectionDetailScreen.vue'
import ProfileScreen from './components/ProfileScreen.vue'
import type { Spot } from './api/spots'
import type { Collection } from './api/collections'

type Tab = 'feed' | 'map' | 'add' | 'lists' | 'profile'

const activeTab         = ref<Tab>('feed')
const selectedSpot      = ref<Spot | null>(null)
const selectedCollection = ref<Collection | null>(null)
const feedKey           = ref(0)

// Clear collection detail when leaving the lists tab
watch(activeTab, () => { selectedCollection.value = null })

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
    <!-- Spot detail overlays everything -->
    <DetailScreen
      v-if="selectedSpot"
      :spot="selectedSpot"
      @back="closeSpot"
      @deleted="onDeleted"
    />

    <!-- Collection detail sits between overlay and tabs -->
    <CollectionDetailScreen
      v-else-if="selectedCollection && activeTab === 'lists'"
      :collection="selectedCollection"
      @back="selectedCollection = null"
      @open-spot="openSpot"
    />

    <template v-else>
      <FeedScreen v-if="activeTab === 'feed'" :key="feedKey" @open="openSpot" />
      <AddScreen v-else-if="activeTab === 'add'" @cancel="activeTab = 'feed'" @saved="activeTab = 'feed'" />
      <CollectionsScreen v-else-if="activeTab === 'lists'" @open="selectedCollection = $event" />
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
