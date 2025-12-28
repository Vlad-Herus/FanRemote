<template>
    <button @click="addDataPoint('April', 50)">lalala2345</button>
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

function addDataPoint(label, newData) {
    try {
        console.log(myCHart.data);
        // Push new label
        labels.push(label);

        tempDataset.data.push(newData);

        //Push new data for each dataset
        // myCHart.data.datasets.forEach((dataset) => {
        //     dataset.data.push(newData);
        // });
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
            // Handle network errors or the error thrown in the .then() block
            console.error('Fetch error:', error.message);
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