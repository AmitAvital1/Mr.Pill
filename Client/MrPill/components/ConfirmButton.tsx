import React from 'react';
import { Text, Pressable, StyleSheet, GestureResponderEvent } from 'react-native';

interface ConfirmButtonProps {

    title: string;
    onPress: (event: GestureResponderEvent) => void;
    isDisabled?: boolean;
    marginTop?: number;
    marginBottom?: number;
    borderWidth?: number;
    borderColor?: string;

  }
  
export const ConfirmButton: React.FC<ConfirmButtonProps> = ({ title, onPress, isDisabled, marginTop, marginBottom, borderWidth, borderColor }) => {

    const buttonStyle = {
        borderWidth: borderWidth? borderWidth : 2,
        borderColor: borderColor? borderColor : "#000",
        marginTop: marginTop? marginTop : 0,
        marginBottom: marginBottom? marginBottom: 0,
        backgroundColor: '#DDFFDD',
        paddingVertical: 10,
        paddingHorizontal: 20,
        borderRadius: 24,
        alignSelf: 'center',
        alignItems: 'center',
        width: 250,
    };

    return (
        <Pressable
        onPress={onPress}
        style={({ pressed }) => [
            buttonStyle,
            isDisabled && styles.buttonDisabled,
            pressed && styles.buttonPressed,
        ]}
        disabled={isDisabled}
        >
        <Text style={styles.buttonText}>{title}</Text>
        </Pressable>
    );
};
  
const styles = StyleSheet.create({

  buttonText: {
    color: '#000',
    fontSize: 32,
    fontWeight: 'bold',
  },
  buttonPressed: {
    backgroundColor: '#FFFFCD',
    borderColor: '#000'
  },
  buttonDisabled: {
    backgroundColor: '#c1c1c1',
    borderColor: '#c1c1c1'
  },

});

export default ConfirmButton;