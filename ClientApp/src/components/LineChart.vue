<template>
    <div class="slider">
        <el-switch v-model="ForceSpeedEnabled" class="ml-2" width="110" inline-prompt active-text="Force Speed"
            inactive-text="Force Speed" @change="onForceSpeedChange" />
        <el-slider v-model="ForceSpeedValue" max="255" :disabled="!ForceSpeedEnabled" show-input
            @change="onForceSpeedChange" />
    </div>
    <canvas ref="chartCanvas"></canvas>
</template>

<script setup>
import { onMounted, ref } from 'vue';
import Chart from 'chart.js/auto'; // 'auto' registers all components automatically

const chartCanvas = ref(null);
const ForceSpeedEnabled = ref(false)
const ForceSpeedValue = ref(0)

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

const onForceSpeedChange = (event) => {
    var speed = null;

    //TODO: this thows when slider is dragged violently to the right beyond the element border
    if (ForceSpeedEnabled.value) {
        speed = ForceSpeedValue.value;
    }
    else
    {
        ForceSpeedValue.value = 0;
    }

    console.log(speed);

    var data = { "forcedSpeed": speed }

    fetch('speed', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
        .then(response => {
            console.log("Speed set to " + speed);

        })
        .catch(error => {
            console.error('Failed to set speed. error: ', error.message);
        });
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

<style scoped>
.slider {
    max-width: 600px;
    display: flex;
    align-items: center;
}

.slider .el-slider {
    margin-top: 0;
    margin-left: 12px;
}

.slider .el-switch {
    margin-right: 20px;
    margin-left: 12px;
}
</style>