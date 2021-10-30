import {Category, File, FormType} from "../../models/File";
import {MultiValue, SingleValue} from "react-select";
import {filesSlice} from "../../redux/filesSlice";
import {AppDispatch} from "../../redux/redux-store";

const optionsCategory: Array<{ value: Category, label: Category }> = [
    {value: 'images', label: 'images'},
    {value: 'links', label: 'links'},
    {value: 'video', label: 'video'},
    {value: 'documents', label: 'documents'},
];
const optionsDate = [
    {value: 'За все время', label: 'За все время'},
    {value: 'Сегодня', label: 'Сегодня'},
    {value: 'Вчера', label: 'Вчера'},
    {value: 'За последние 7 дней', label: 'За последние 7 дней'},
    {value: 'За последние 30 дней', label: 'За последние 30 дней'},
    {value: '', label: 'Другой период...'}
];

export const configFilters = (filesData: File[]) => {
    const optionsName = filesData.map((f) => ({label: f.fileName, value: f.fileName}));
    const optionsSender = filesData.map((f) => ({label: f.senderId.toString(), value: f.senderId}));
    const optionsChat = filesData.map((f) => ({label: f.chatId.toString(), value: f.chatId}));
    return {optionsName, optionsSender, optionsDate, optionsCategory, optionsChat};
}

// @ts-nocheck
export const EventsChange = (dispatch: AppDispatch, actions: typeof filesSlice.actions) => {
    const onChangeDate = (e: SingleValue<FormType<string>>) => dispatch(actions.changeFilterDate(e?.value));
    const onChangeSenders = (e: MultiValue<FormType<number>>) => dispatch(actions.changeFilterSenders(e.map((v => v?.value))));
    const onChangeChats = (e: MultiValue<FormType<number>>) => dispatch(actions.changeFilterChats(e.map((v => v?.value))));
    const onChangeFileName = (e: SingleValue<FormType<number>>) => {
        // @ts-ignore
        dispatch(actions.changeFilterFileName(e?.label))
    };
    const onChangeCategories = (e: MultiValue<FormType<Category>>) => {
        dispatch(actions.changeFilterCategories(e.map((v => v?.value))));
    }
    return {onChangeFileName,onChangeDate, onChangeSenders, onChangeChats, onChangeCategories};
};
