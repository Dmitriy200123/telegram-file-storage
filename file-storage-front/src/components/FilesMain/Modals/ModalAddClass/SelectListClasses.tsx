import React, {memo} from 'react';
import "../Modal.scss"
import {ClassificationType} from "../../../../models/Classification";
import {ReactComponent as CheckIcon} from "./../../../../assets/check.svg";
import classes from "./ModalAddClass.module.scss";
import classNames from "classnames";

type PropsType = {
    classifications: ClassificationType[],
    classActiveId?: string
    onClick: (classId: string) => void
}

const Classifications: React.FC<PropsType> = memo(({classifications, onClick, classActiveId}) => {

    return (<>
            {classifications?.map((e) => {
                return <div
                    className={classNames(classes.classItem, classActiveId === e.id && classes.classItem_active)}
                    key={e.id} onClick={() => onClick(e.id)}>
                    <CheckIcon />
                    <span>{e.name}</span>
                </div>
            })}
        </>

    )
})

export default Classifications;