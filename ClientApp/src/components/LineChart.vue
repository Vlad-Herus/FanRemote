<template>
    <canvas ref="chartCanvas"></canvas>
</template>

<script setup>
import { onMounted, ref } from 'vue';
import Chart from 'chart.js/auto'; // 'auto' registers all components automatically

const chartCanvas = ref(null);

var myCHart = null;
var eTag = null;
var labels = [];
var tempDataset = { label: 'Temp', data: [] };
var targetDataset = { label: 'Target', data: [] };
var speedDataset = { label: 'Speed', data: [] };

onMounted(() => {
    myCHart = new Chart(chartCanvas.value, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [
                tempDataset,
                targetDataset,
                speedDataset
            ]
        },
        options: { responsive: true }
    });

    myCHart.data.datasets.forEach((dataset) => {
        if (dataset.label === 'Temp')
            tempDataset = dataset;
    });
});

function pollPid() {
    fetch('data', {
        method: 'GET',
        headers: {
            'ETag': eTag
        },
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            eTag = response.headers.get('etag');
            return response.json();
        })
        .then(data => {
            data.sort((a, b) => new Date(a.timestamp).getTime() > new Date(b.timestamp).getTime());
            data.forEach(item => {
                var date = new Date(item.timestamp);
                var minutes = String(date.getMinutes()).padStart(2, '0');
                var seconds = String(date.getSeconds()).padStart(2, '0');
                labels.push(minutes + ':' + seconds);
                tempDataset.data.push(item.temp);
                targetDataset.data.push(item.target);
                speedDataset.data.push(item.speed);
                myCHart.update();
            });
        })
        .catch(error => {
            console.error('Failed to fetch Temp data. error: ', error.message);
        });

    setTimeout(pollPid, 1000); // Delay of 0 ms puts it at the end of the current call stack
}
pollPid();
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
    },
    beforeUnmount() {
        if (this.myChart) {
            this.myChart.destroy();
        }
    },
    methods: {
    }
}
</script>

<style scoped>
</style>