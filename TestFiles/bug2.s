    LDR R0, =DATA       @ works fine R0=0x101c, R1=?
    LDRH R1, [R0]       @ works fine R0=0x101c, R1=0x0000FFFF
    ADD R0, R0, #2      @ works fine R0=0x101e, R1=0x0000FFFF
    LDRH R1, [R0]       @ works fine R0=0x101e, R1=0x00000011
    SUB R0, R0, #2      @ works fine R0=0x101c, R1=0x00000011
    LDRSH R1, [R0]      @ wrong!     R0=0xFFFF0e5c, R1=0x0000FFFF
                        @ expected   R0=0x0000101c, R1=0xFFFFFFFF
END:
    B END    

DATA:
    .hword 0xFFFF, 0x0011
