import React from 'react';
import "./FilesMain.scss"
import {Category} from "../../models/File";
import { Link } from 'react-router-dom';

const FragmentFile: React.FC<PropsType> = ({fileId, fileName, uploadDate, fileType, senderId, chatId}) => {
    return <React.Fragment key={fileId}>
        <Link className={"files__item files__item_name"} to={"/file"} replace>{fileName}</Link>
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


