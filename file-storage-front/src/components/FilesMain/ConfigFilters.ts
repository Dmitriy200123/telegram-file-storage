import {Category, Chat, Sender, TypeFile} from "../../models/File";

export const optionsCategory: Array<{ value: Category, label: string }> = [
    {value: Category.images, label: 'Картинки'},
    {value: Category.links, label: 'Ссылки'},
    {value: Category.video, label: 'Видео'},
    {value: Category.documents, label: 'Документы'},
];
const optionsDate = [
    {value: 'За все время', label: 'За все время'},
    {value: 'Сегодня', label: 'Сегодня'},
    {value: 'Вчера', label: 'Вчера'},
    {value: 'За последние 7 дней', label: 'За последние 7 дней'},
    {value: 'За последние 30 дней', label: 'За последние 30 дней'},
    {value: '1', label: 'Другой период...'}
];

export const configFilters = (filesData: TypeFile[], chats:Chat[] | null, senders:Sender[] | null) => {
    const optionsName = filesData.map((f) => ({label: f.fileName, value: f.fileName}));
    const optionsSender = senders?.map((f) => ({label: f.fullName, value: f.id}));
    const optionsChat = chats ? chats.map((f) => ({label: f.name, value: f.id})) : [];
    return {optionsName, optionsSender, optionsDate, optionsCategory, optionsChat};
}

