import { Link } from "react-router-dom";
import "./table.scss"

const Table = ({ children, resource, header }) => {



    return (
        <>
            <div className="flex justify-between">
                <h1 className="text-5xl font-semibold">{resource} list</h1>
                {
                    resource !== "user" &&
                    <Link to="create" className="w-44 flex justify-center items-center bg-blue-700 text-white rounded-md">Add {resource}</Link>
                }
            </div>
            <div className="relative overflow-x-auto shadow-md sm:rounded-lg mt-10">
                <table className="w-full capitalize text-sm text-left rtl:text-right text-gray-500 ">
                    <thead className="text-xs text-gray-700 uppercase bg-gray-50 ">
                        <tr>
                            {header.map((item, i) =>
                                <th key={i} scope="col" className="text-center px-6 py-3">
                                    {item?.name}
                                </th>
                            )}
                            <th colSpan={2} className="text-center">actions</th>
                        </tr>
                    </thead>
                    {children}
                </table >
            </div >
        </>

    )
}

export default Table;



