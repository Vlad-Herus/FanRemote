<script setup>

</script>

<template>
  <h1>You did it!</h1>
  <p>
    Visit <a href="https://vuejs.org/" target="_blank" rel="noopener">vuejs.org</a> to read the
    documentation
  </p>
  <div id="lineDiv" style="width: 80%;">
    <LineChart />
  </div>

</template>

<script>
import LineChart from './components/LineChart.vue'

export default {
  name: 'App',
  components: { LineChart }
}
</script>

<style scoped></style>
