import { Text } from "react-native"

// converts string to react simple react text Function Component
export const strFC = (str: string) => {
    return () => {return (<Text style={{fontSize: 18, fontWeight: 'bold',}}>{str}</Text>)};
};