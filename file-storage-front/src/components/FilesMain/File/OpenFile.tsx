import React, {FC, memo} from 'react';
import "./File.scss"
import {ReactComponent as Svg} from "../../../assets/download.svg";
import {useAppDispatch} from "../../../utils/hooks/reduxHooks";
import {ExpandingObject, ModalContent, Rights, TypeFile} from "../../../models/File";
import {Link} from 'react-router-dom';
import {ReactComponent as Edit} from "../../../assets/edit.svg";
import {ReactComponent as Delete} from "../../../assets/delete.svg";
import {filesSlice} from "../../../redux/filesSlice";
import {Button} from "../../utils/Button/Button";
import {ReactComponent as Tag} from "../../../assets/tag.svg";
import {deleteClassificationDocument} from "../../../redux/thunks/fileThunks";
import {ReactComponent as Reject} from "../../../assets/reject.svg";

const {openModal} = filesSlice.actions;

type PropsType = {
    id: string,
    file: TypeFile & { message?: string },
    rights: Rights[],
    filesTypes: ExpandingObject<string>,
    urlPreview?: string | null
};


const OpenedFile: React.FC<PropsType> = memo(({id, file, rights, filesTypes, urlPreview}) => {
    const dispatch = useAppDispatch();
    const {fileName, fileType, sender, chat, uploadDate, message, url, classification} = file;
    const canRename = rights?.includes(Rights["Переименовывать файлы"]);

    function openRename() {
        dispatch(openModal({id: id, content: ModalContent.Edit}));
    }

    function openAddClass() {
        dispatch(openModal({id, content: ModalContent.AddClass}));
    }

    function removeClass() {
        if (!classification)
            return;
        dispatch(deleteClassificationDocument({documentId: id, classId: classification.id}))
    }

    function onDownload() {
        if (url)
            window.open(url);
    }

    return (
        <div className={"file"}>
            <div className="file__header">
                <h2 className="file__title">Файл</h2>
                <Link className="file__close" to={"/files"}/>
            </div>
            <div className="file__content">
                <h3 className="file__content-title"
                    onClick={canRename ? openRename : undefined}>
                    <span className={"file__content-title-text"}>{fileName}</span> {canRename && <Edit/>}</h3>
                {+fileType === 6 && <div className={"file__classes"}>
                    <div className={"file__classItem"} onClick={openAddClass}>
                        <Tag/><span>Присвоить классификацию</span></div>
                    {classification && <div className={"file__classItem"}
                                            onClick={removeClass}>
                        <Reject/><span>Отозвать классфикацию</span></div>}
                </div>}
                <section className="file__contentTable">
                    <div className="file__item"><span>Формат: </span></div>
                    <div className="file__item">{filesTypes && filesTypes[fileType]}</div>
                    {classification && <>
                        <div className="file__item"><span>Классификация: </span></div>
                        <div className="file__item "><span
                            className="file__color">{classification.name}</span></div>
                    </>}
                    <div className="file__item"><span>Отправитель: </span></div>
                    <div className="file__item"><a>{sender?.fullName}</a></div>
                    <div className="file__item"><span>Чат: </span></div>
                    <div className="file__item"><a>{chat?.name}</a></div>
                    <div className="file__item"><span>Дата отправки: </span></div>
                    <div className="file__item">{uploadDate}</div>
                </section>
                <div className={"file__btns"}>
                    {+fileType !== 5 && +fileType !== 4 &&
                    <Button className="file__btn" onClick={onDownload} disabled={!file.url}>
                        <div>Скачать файл</div>
                        <Svg/>
                    </Button>}
                    {rights?.includes(Rights["Удалять файлы"]) &&
                    <Button
                        onClick={() => dispatch(() => dispatch(openModal({id: id, content: ModalContent.Remove})))}
                        type={"danger"} className={"file__btn_delete"}><span>Удалить</span><Delete/></Button>}
                </div>
                {(message || urlPreview) && <h2 className={"file__previewTitle"}>Соодержимое:</h2>}
                {message && <div className="file__message">
                    {+fileType === 4 ? <a href={message}>{message} </a> : message}
                </div>}
                {+fileType !== 5 && +fileType !== 4 && urlPreview && <Embed urlPreview={urlPreview} type={+fileType}/>}
            </div>
        </div>
    );
});

const Embed: FC<{ type: number, urlPreview: string }> = ({type, urlPreview}) => {
    if (type === 3)
        return <img alt={"image"} className={"file__embed"} src={urlPreview} width="100%"/>
    if (type === 2 || type === 1)
        return <embed src={urlPreview}/>

    return <embed src={urlPreview} className={"file__embed"}/>
}

export default OpenedFile;




