import React from 'react';
import "./FilesMain.scss"
import {Category} from "../../types/types";

const FragmentFile: React.FC<PropsType> = ({fileId, fileName, uploadDate, fileType, senderId, chatId}) => {
    return <React.Fragment key={fileId}>
        <div className={"files__item"}>{fileName}</div>
        <div className={"files__item"}>{uploadDate}</div>
        <div className={"files__item"}>{fileType}</div>
        <div className={"files__item"}>{senderId}</div>
        <div className={"files__item"}>{chatId}</div>
    </React.Fragment>
};

type PropsType = {
    fileId: number,
    fileName: string,
    uploadDate: string,
    fileType: Category,
    senderId: number,
    chatId: number,
};

export default FragmentFile;


