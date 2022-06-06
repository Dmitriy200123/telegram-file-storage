import React, {FC, useEffect} from "react";
import {Select} from "../../utils/Inputs/Select";
import {useAppDispatch, useAppSelector} from "../../../utils/hooks/reduxHooks";
import {fetchAllClassifications} from "../../../redux/classesDocs/classesDocsThunks";

type PropsType = {
    setValueForm: (name: string, value: any) => void,
    values?: string[] | null
}

const ClassificationFilter: FC<PropsType> = ({setValueForm, values}) => {
    const dispatch = useAppDispatch();
    const classifications = useAppSelector((state) => state.classesDocs.classifications);
    const options = classifications?.map((e) => ({value: e.id, label: e.name}));
    useEffect(() => {
        if (!classifications || classifications.length === 0)
            dispatch(fetchAllClassifications());
    }, []);
    options?.unshift({value: '00000000-0000-0000-0000-000000000000', label: 'Без классификации'});
    
    return (<Select name={"classificationIds"} className={"files__filter files__filter_select"}
                    setValue={setValueForm} placeholder={"Классификация"}
                    values={values} options={options} isMulti={true}/>)
}

export default ClassificationFilter;