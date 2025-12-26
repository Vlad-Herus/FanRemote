<template>
    <button @click="addDataPoint('April', 50)">lalala2345</button>
    <canvas ref="chartCanvas"></canvas>
</template>

<script setup>
import { onMounted, ref } from 'vue';
import Chart from 'chart.js/auto'; // 'auto' registers all components automatically

const chartCanvas = ref(null);
var myCHart = null;

onMounted(() => {
    myCHart = new Chart(chartCanvas.value, {
        type: 'bar',
        data: {
            labels: ['Red', 'Blue', 'Yellow'],
            datasets: [{ label: 'Votes', data: [12, 19, 3] }]
        },
        options: { responsive: true }
    });
});

function addDataPoint(label, newData) {
    try {
        console.log(myCHart.data);
        // Push new label
        myCHart.data.labels.push(label);

        // Push new data for each dataset
        myCHart.data.datasets.forEach((dataset) => {
            dataset.data.push(newData);
        });
        myCHart.update();
        //refreshMyChart();
        // Important: Tell Chart.js to update the chart
        // If you are using raw Chart.js instance (e.g., this.myChart)
        // you would call this.myChart.update();
        // With vue-chartjs, ensure the component re-renders when chartData changes.
        // The reactiveProp feature in older versions used a watcher on the whole object.
        // In newer versions with Vue 3, simply mutating the reactive data works.
    }
    catch (error) {
        console.log(error);
    }
}

</script>

<script>

export default {
    name: 'LineChart',
    props: {
        chartData: {
            type: Object,
            required: true
        },
        chartOptions: {
            type: Object,
            default: () => ({})
        }
    },
    mounted() {
        // const ctx = this.$refs.chartCanvas.getContext('2d');
        // this.myChart = new Chart(ctx, {
        //     type: this.chartData.type, // e.g., 'bar', 'line', 'pie'
        //     data: this.chartData.data,
        //     options: this.chartOptions
        // });
        // new Chart(chartCanvas.value, {
        //     type: 'bar',
        //     data: {
        //         labels: ['Red', 'Blue', 'Yel\low'],
        //         datasets: [{ label: 'Votes', data: [12, 19, 3] }]
        //     },
        //     options: { responsive: true }
        // });
    },
    // Optional: Add a beforeDestroy or onUnmounted hook to clean up the chart instance
    beforeUnmount() {
        if (this.myChart) {
            this.myChart.destroy();
        }
    },
    methods: {
        refreshMyChart() {
            this.chartData = {
                ...this.chartData
            };
        }
    }
}
</script>