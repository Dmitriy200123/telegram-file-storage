import React, {FC, useState} from 'react';
import "./File.scss"
import Document from "./../../../assets/document.png";
import Modal from "../../utils/Modal/Modal";
import classNames from "classnames";

type PropsType = { urlPreview?: string | null, fileType: number };

const Img: FC<PropsType> = ({urlPreview, fileType}) => {
    const [isOpen, setIsOpen] = useState(false);
    const isImg = urlPreview && fileType === 3;
    const src = isImg ? urlPreview : Document;

    return <>
        <img src={src} className={classNames("file__img", !isImg && "file__img_default")}
             alt={"type"}
             onClick={() => setIsOpen(true)}/>
        {isOpen && <Modal className={"file__modalPreview"} onOutsideClick={() => setIsOpen(false)}>
            <img src={src} alt={"type"}/>
        </Modal>}
    </>
}

export default Img;




