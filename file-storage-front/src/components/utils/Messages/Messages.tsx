import React from 'react';
import "./Messages.scss";
import {MessageType, MessageTypeEnum} from "../../../models/File";
import {useDispatch} from "react-redux";
import {useAppDispatch} from "../../../utils/hooks/reduxHooks";
import {filesSlice} from "../../../redux/filesSlice";

const {clearError} = filesSlice.actions;
export const Messages: React.FC<{ className?: string, messages: Array<MessageType> }> = ({messages, className}) => {
    const messagesUi = messages.map(({value, type}, index) => <Message key={index} type={type} value={value}
                                                                       index={index}/>);

    return (
        <section className={(`${className} ` ?? "") + "messages"}>
            {messagesUi}
        </section>
    )
}

const Message: React.FC<{ value: string, type: MessageTypeEnum, index: Number }> = ({value, type, index}) => {
    const dispatch = useAppDispatch();
    const dict = {
        [MessageTypeEnum.Error]: {
            text: "Проблема",
            icon: null,
        },
        [MessageTypeEnum.Message]: {
            text: "Успешно",
            icon: null,
        }
    }

    function onClose() {
        dispatch(clearError(index));
    }

    return (
        <div className={"message"}>
            <h2>{dict[type].text}</h2>
            <p>{value}</p>
            <button onClick={onClose}>закрыть</button>
        </div>
    )
}


