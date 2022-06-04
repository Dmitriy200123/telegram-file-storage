import React, {FC, useState} from 'react';
import {Button} from "../../../utils/Button/Button";
import {InputText} from "../../../utils/Inputs/Text/InputText";
import classes from "../Modal.module.scss";
import {useAppDispatch} from "../../../../utils/hooks/reduxHooks";
import {fetchRenameClassification} from "../../../../redux/classesDocs/classesDocsThunks";

type PropsType = {
    onOutsideClick?: () => void
    args: {id:string, name: string}
}


const Edit: FC<PropsType> = ({onOutsideClick, args}) => {
    const [name, changeName] = useState(args.name);
    const dispatch = useAppDispatch();
    function onChange(e: any) {
        changeName(e.target.value);
    }

    function onSubmit() {
        if (name.length === 0)
            return;
        dispatch(fetchRenameClassification({name: name, id: args.id}));
    }

    return (
        <div className={classes.block}>
            <h2 className={classes.title}>Переименовать</h2>
            <InputText value={name} onChange={onChange}/>
            <div className={classes.btns}>
                <Button onClick={onSubmit} disabled={name.length === 0 || name === args.name}>ОК</Button>
                <Button type={"transparent"} onClick={onOutsideClick}>ОТМЕНА</Button>
            </div>
        </div>
    );
};


export default Edit;
