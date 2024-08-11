import { Text } from "react-native"

// converts string to react simple react text Function Component
export const strFC = (str: string, size?: number) => {
    return () => {return (<Text style={{fontSize: size? size: 18, fontWeight: 'bold',}}>{str}</Text>)};
};