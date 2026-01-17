<template>
    <div class="slider">
        <el-switch v-model="ForceSpeedEnabled" class="ml-2" width="110" inline-prompt active-text="Force Speed"
            inactive-text="Force Speed" @change="onForceSpeedChange" />
        <el-slider v-model="ForceSpeedValue" max="255" :disabled="!ForceSpeedEnabled" show-input
            @change="onForceSpeedChange" />
    </div>
</template>


<script setup>
import { onMounted, ref } from 'vue';

const ForceSpeedEnabled = ref(false)
const ForceSpeedValue = ref(0)

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
</script>

<script>

export default {
    name: 'SpeedOverride',
    props: {
    },
    mounted() {
    },
    beforeUnmount() {
    },
    methods: {
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