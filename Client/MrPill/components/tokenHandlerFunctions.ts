import * as FileSystem from 'expo-file-system';

export const saveTokenToFile = async (content: string) => {
    const hiddenDataFolder = `${FileSystem.documentDirectory}.hiddenDataFolder/`;

    const folderInfo = await FileSystem.getInfoAsync(hiddenDataFolder);
    if (!folderInfo.exists) {
        await FileSystem.makeDirectoryAsync(hiddenDataFolder);
    }

    const filePath = `${hiddenDataFolder}${"userToken"}`;

    try {
        await FileSystem.writeAsStringAsync(filePath, content, {
            encoding: FileSystem.EncodingType.UTF8,
        });
        console.log('File written successfully');
    } catch (error) {
        console.error('Error writing file:', error);
    }
};

export const readTokenFromFile = async (): Promise<string> => {
    const hiddenDataFolder = `${FileSystem.documentDirectory}.hiddenDataFolder/`;
    const filePath = `${hiddenDataFolder}${"userToken"}`;

    try {
        const content = await FileSystem.readAsStringAsync(filePath, {
            encoding: FileSystem.EncodingType.UTF8,
        });
        console.log('File read successfully');
        return content;
    } catch (error) {
        console.error('Error reading file:', error);
        throw error;
    }
};