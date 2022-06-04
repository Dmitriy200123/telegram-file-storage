import React, {useEffect} from 'react';
import "./Messages.scss";
import {MessageType, MessageTypeEnum} from "../../../models/File";
import {useAppDispatch} from "../../../utils/hooks/reduxHooks";
import {profileSlice} from "../../../redux/profileSlice";
import error from "../../../assets/error.svg";
import success from "../../../assets/success.svg";

const {clearMessage} = profileSlice.actions;
type PropsType = { className?: string, messages: Array<MessageType> };
export const Messages: React.FC<PropsType> = ({messages, className}) => {
    const messagesUi = messages.map(({value, type}, index) => <Message key={index} type={type} value={value}
                                                                       index={index}/>);
    return (
        <section className={(`${className} ` ?? "") + "messages"}>
            {messagesUi}
        </section>
    )
}

const messagesConfig = {
    [MessageTypeEnum.Error]: {
        text: "Проблема",
        className: "color-error",
        icon: error,
    },
    [MessageTypeEnum.Message]: {
        className: "color-success",
        text: "Успешно",
        icon: success,
    }
}

const Message: React.FC<{ value: string, type: MessageTypeEnum, index: Number }> = ({value, type, index}) => {
    const dispatch = useAppDispatch();
    const currentConfig = messagesConfig[type];
    function onClose() {
        dispatch(clearMessage(index));
    }

    useEffect(() => {
        if (MessageTypeEnum.Message === type) {
            setTimeout(() => onClose(), 1500);
        }
    },[type])

    return (
        <div className={"message " + currentConfig.className}>
            <img src={currentConfig.icon}/>
            <h2>
                {currentConfig.text}
            </h2>
            <p className={"p-padding"}>{value}</p>
            <button className={"close-button"} onClick={onClose}/>
        </div>
    )
}



