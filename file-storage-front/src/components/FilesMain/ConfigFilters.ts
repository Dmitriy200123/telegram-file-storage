import { Chat, Sender, TypeFile} from "../../models/File";


const optionsDate = [
    {value: 'За все время', label: 'За все время'},
    {value: 'Сегодня', label: 'Сегодня'},
    {value: 'Вчера', label: 'Вчера'},
    {value: 'За последние 7 дней', label: 'За последние 7 дней'},
    {value: 'За последние 30 дней', label: 'За последние 30 дней'},
    {value: '1', label: 'Другой период...'}
];

export const configFilters = (filesNames: string[] | null, chats:Chat[] | null, senders:Sender[] | null) => {
    const optionsName = Array.from(new Set(filesNames))?.map((f) => ({label: f, value: f}));
    const optionsSender = senders?.map((f) => ({label: f.fullName, value: f.id}));
    const optionsChat = chats ? chats.map((f) => ({label: f.name, value: f.id})) : [];
    return {optionsName, optionsSender, optionsDate, optionsChat};
}

