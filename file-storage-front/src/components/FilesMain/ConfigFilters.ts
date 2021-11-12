import {Category, Chat, File} from "../../models/File";

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
    {value: '1', label: 'Другой период...'}
];

export const configFilters = (filesData: File[], chats:Chat[] | null) => {
    const optionsName = filesData.map((f) => ({label: f.fileName, value: f.fileName}));
    const optionsSender = filesData.map((f) => ({label: f.senderId.toString(), value: f.senderId}));
    const optionsChat = chats ? chats.map((f) => ({label: f.id, value: f.name})) : [];
    return {optionsName, optionsSender, optionsDate, optionsCategory, optionsChat};
}

