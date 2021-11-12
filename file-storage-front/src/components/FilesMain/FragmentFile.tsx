import React from 'react';
import "./FilesMain.scss"
import {Category, File} from "../../models/File";
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

type PropsType = File;

export default FragmentFile;


