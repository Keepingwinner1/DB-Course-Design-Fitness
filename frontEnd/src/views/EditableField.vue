<template>
    <div class="editable-field">
        <label>{{ label }}</label>
        <div class="field-container" :class="{ editable: isEditable }">
            <input v-if="type === 'input'" :value="currentValue" :readonly="!isEditable"
                @input="updateValue($event.target.value)" @blur="handleBlur"
                :style="{ backgroundColor: isEditable ? 'white' : 'transparent' }" />
            <select v-if="type === 'select'" v-model="currentValue" @blur="handleBlur"
                :style="{ backgroundColor: isEditable ? 'white' : 'transparent' }">
                <option v-for="option in options" :key="option" :value="option">{{ option }}</option>
            </select>
            <span @click="toggleEdit" class="edit-icon">&#9998;</span>
        </div>
    </div>
</template>

<script>
export default {
    props: {
        label: String,
        value: [String, Number],
        type: String,
        options: Array,
    },
    data() {
        return {
            isEditable: false,
            currentValue: this.value,
            initialValue: this.value,  // 保存初始值
        };
    },
    methods: {
        toggleEdit() {
            if (this.isEditable) {
                this.handleBlur();
            } else {
                this.isEditable = true;
                this.initialValue = this.currentValue; // 进入编辑模式时记录当前值
            }
        },
        handleBlur() {
            if (this.currentValue !== this.initialValue || this.isEditable) {
                // 即使没有修改内容，也触发保存逻辑并恢复透明背景
                this.isEditable = false;
                this.$emit('save', this.currentValue);
            } else {
                // 如果没有任何改变，仍然触发保存逻辑并恢复透明背景
                this.isEditable = false;
                this.$emit('save', this.currentValue);
            }
        },
        updateValue(newValue) {
            this.currentValue = newValue;
            this.$emit('input', this.currentValue);
        }
    },
    watch: {
        value(newValue) {
            this.currentValue = newValue;
        },
    },
};
</script>

<style scoped>
.editable-field {
    position: relative;
    margin-bottom: 20px;
    flex-grow: 1;
}

.field-container {
    display: flex;
    align-items: center;
    background-color: transparent;
    border-radius: 4px;
}

.field-container.editable {
    background-color: transparent;
}

.edit-icon {
    position: absolute;
    right: 10px;
    cursor: pointer;
}

input,
select {
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 4px;
    height: 50px;
    font-size: 16px;
    width: 100%;
    background-color: transparent;
}
</style>
