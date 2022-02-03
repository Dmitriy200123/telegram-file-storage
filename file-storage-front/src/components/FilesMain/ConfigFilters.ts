import {Chat, Sender} from "../../models/File";

export const configFilters = (filesNames: string[] | null, chats:Chat[] | null, senders:Sender[] | null) => {
    const optionsName = Array.from(new Set(filesNames))?.map((f) => ({label: f, value: f}));
    const optionsSender = senders?.map((f) => ({label: f.fullName, value: f.id}));
    const optionsChat = chats ? chats.map((f) => ({label: f.name, value: f.id})) : [];
    return {optionsName, optionsSender, optionsChat};
}

