<script setup>

</script>

<template>
  <h1>You did it!</h1>
  <p>
    Visit <a href="https://vuejs.org/" target="_blank" rel="noopener">vuejs.org</a> to read the
    documentation
  </p>
  <div id="lineDiv" style="width: 80%;">
    <SpeedOverride />
    <LineChart />
  </div>

</template>

<script>
import LineChart from './components/LineChart.vue'
import SpeedOverride from './components/SpeedOverride.vue';

export default {
  name: 'App',
  components: { LineChart, SpeedOverride }
}
</script>

<style scoped></style>
